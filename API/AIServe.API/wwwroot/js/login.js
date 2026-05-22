const API_BASE = '/ApiService';

document.addEventListener('DOMContentLoaded', () => {
    initTabs();
    initForms();
    checkAuth();
});

function initTabs() {
    document.querySelectorAll('.form-tab').forEach(tab => {
        tab.addEventListener('click', () => {
            document.querySelectorAll('.form-tab').forEach(t => t.classList.remove('active'));
            document.querySelectorAll('.form-content').forEach(c => c.classList.remove('active'));
            tab.classList.add('active');
            document.getElementById(`${tab.dataset.form}-form`).classList.add('active');
        });
    });
}

function initForms() {
    document.getElementById('login-form').addEventListener('submit', handleLogin);
    document.getElementById('register-form').addEventListener('submit', handleRegister);
}

async function checkAuth() {
    const token = localStorage.getItem('authToken');
    const username = localStorage.getItem('username');
    if (token && username) {
        window.location.href = 'index.html';
    }
}

async function request(action, options = {}) {
    try {
        const url = new URL(`${API_BASE}`, window.location.origin);
        url.searchParams.set('action', action);
        for (let key in options.query) {
            url.searchParams.set(key, options.query[key]);
        }

        const response = await fetch(url.toString(), {
            headers: { 'Content-Type': 'application/json', ...options.headers },
            ...options
        });
        return await response.json();
    } catch (error) {
        showToast('请求失败: ' + error.message, 'error');
        return null;
    }
}

async function handleLogin(e) {
    e.preventDefault();

    const username = document.getElementById('login-username').value.trim();
    const password = document.getElementById('login-password').value;

    if (!username || !password) {
        showToast('请输入用户名和密码', 'warning');
        return;
    }

    const result = await request('login_login', {
        method: 'POST',
        body: JSON.stringify({ username, password })
    });

    if (result && result.success) {
        localStorage.setItem('authToken', result.data.token);
        localStorage.setItem('username', result.data.username);
        showToast('登录成功', 'success');
        setTimeout(() => {
            window.location.href = 'index.html';
        }, 500);
    } else {
        showToast(result?.message || '登录失败', 'error');
    }
}

async function handleRegister(e) {
    e.preventDefault();

    const username = document.getElementById('register-username').value.trim();
    const password = document.getElementById('register-password').value;
    const confirm = document.getElementById('register-confirm').value;

    if (!username || !password) {
        showToast('请输入用户名和密码', 'warning');
        return;
    }

    if (password !== confirm) {
        showToast('两次输入的密码不一致', 'warning');
        return;
    }

    const result = await request('login_register', {
        method: 'POST',
        body: JSON.stringify({ username, password })
    });

    if (result && result.success) {
        showToast('注册成功，请登录', 'success');
        document.querySelector('[data-form="login"]').click();
        document.getElementById('login-username').value = username;
        document.getElementById('register-username').value = '';
        document.getElementById('register-password').value = '';
        document.getElementById('register-confirm').value = '';
    } else {
        showToast(result?.message || '注册失败', 'error');
    }
}

function showToast(message, type = 'info') {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = `toast show ${type}`;
    setTimeout(() => toast.classList.remove('show'), 3000);
}
