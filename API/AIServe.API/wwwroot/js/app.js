const API_BASE = '/ApiService';

const ServiceTypeMap = { 1: '咨询服务', 2: '保养服务', 3: '维修服务', 4: '其他服务' };
const StatusMap = { 1: '待确认', 2: '已确认', 3: '已完成', 4: '已取消' };
const StatusClassMap = { 1: 'status-pending', 2: 'status-confirmed', 3: 'status-completed', 4: 'status-cancelled' };

let currentEditId = null;

document.addEventListener('DOMContentLoaded', () => {
    if (!checkAuth()) return;
    initNavigation();
    loadReservations();
    loadConfig();
    document.getElementById('user-info').textContent = `欢迎, ${localStorage.getItem('username')}`;
});

function checkAuth() {
    const token = localStorage.getItem('authToken');
    const username = localStorage.getItem('username');
    if (!token || !username) {
        window.location.href = 'login.html';
        return false;
    }
    return true;
}

async function handleLogout() {
    const token = localStorage.getItem('authToken');
    if (token) {
        await request('login_logout', {
            method: 'POST',
            body: JSON.stringify({ token })
        });
    }
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
    window.location.href = 'login.html';
}

function initNavigation() {
    document.querySelectorAll('.nav-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.nav-btn').forEach(b => b.classList.remove('active'));
            document.querySelectorAll('.page').forEach(p => p.classList.remove('active'));
            btn.classList.add('active');
            document.getElementById(`${btn.dataset.page}-page`).classList.add('active');
        });
    });
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

async function loadReservations() {
    const tbody = document.getElementById('reservation-tbody');
    tbody.innerHTML = '<tr><td colspan="7" class="loading">加载中...</td></tr>';

    const result = await request('reservation_list');
    if (!result || !result.success) {
        tbody.innerHTML = '<tr><td colspan="7" class="empty">加载失败</td></tr>';
        showToast(result?.message || '加载失败', 'error');
        return;
    }

    const list = result.data || [];
    if (list.length === 0) {
        tbody.innerHTML = '<tr><td colspan="7" class="empty">暂无数据</td></tr>';
        return;
    }

    tbody.innerHTML = list.map(r => `
        <tr>
            <td>${r.id}</td>
            <td>${r.customerName}</td>
            <td>${r.phone}</td>
            <td>${formatDate(r.reservationTime)}</td>
            <td>${ServiceTypeMap[r.serviceType] || '-'}</td>
            <td><span class="status-badge ${StatusClassMap[r.status]}">${StatusMap[r.status] || '-'}</span></td>
            <td class="actions">
                <button class="btn btn-small" onclick="editReservation(${r.id})">编辑</button>
                <button class="btn btn-small btn-success" onclick="updateStatus(${r.id}, 2)">确认</button>
                <button class="btn btn-small btn-danger" onclick="deleteReservation(${r.id})">删除</button>
            </td>
        </tr>
    `).join('');
}

async function showAddModal() {
    currentEditId = null;
    document.getElementById('modal-title').textContent = '新增预约';
    document.getElementById('customer-name').value = '';
    document.getElementById('customer-phone').value = '';
    document.getElementById('reservation-time').value = '';
    document.getElementById('service-type').value = '1';
    document.getElementById('remark').value = '';
    document.getElementById('modal').classList.add('show');
}

async function editReservation(id) {
    const result = await request('reservation_get', { query: { id } });
    if (!result || !result.success) {
        showToast('获取预约信息失败', 'error');
        return;
    }

    const r = result.data;
    currentEditId = id;
    document.getElementById('modal-title').textContent = '编辑预约';
    document.getElementById('customer-name').value = r.customerName;
    document.getElementById('customer-phone').value = r.phone;
    document.getElementById('reservation-time').value = formatDateTimeLocal(r.reservationTime);
    document.getElementById('service-type').value = r.serviceType;
    document.getElementById('remark').value = r.remark || '';
    document.getElementById('modal').classList.add('show');
}

function closeModal() {
    document.getElementById('modal').classList.remove('show');
}

async function saveReservation() {
    const data = {
        id: currentEditId || 0,
        customerName: document.getElementById('customer-name').value,
        phone: document.getElementById('customer-phone').value,
        reservationTime: document.getElementById('reservation-time').value,
        serviceType: parseInt(document.getElementById('service-type').value),
        remark: document.getElementById('remark').value
    };

    if (!data.customerName || !data.phone || !data.reservationTime) {
        showToast('请填写必填项', 'warning');
        return;
    }

    let result;
    if (currentEditId) {
        result = await request('reservation_update', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    } else {
        result = await request('reservation_create', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    if (result && result.success) {
        showToast(currentEditId ? '更新成功' : '创建成功', 'success');
        closeModal();
        loadReservations();
    } else {
        showToast(result?.message || '操作失败', 'error');
    }
}

async function updateStatus(id, status) {
    const result = await request('reservation_updatestatus', {
        method: 'POST',
        body: JSON.stringify({ id, status })
    });

    if (result && result.success) {
        showToast('状态更新成功', 'success');
        loadReservations();
    } else {
        showToast(result?.message || '操作失败', 'error');
    }
}

async function deleteReservation(id) {
    if (!confirm('确定要删除这条预约记录吗？')) return;

    const result = await request('reservation_delete', { query: { id } });
    if (result && result.success) {
        showToast('删除成功', 'success');
        loadReservations();
    } else {
        showToast(result?.message || '删除失败', 'error');
    }
}

async function loadConfig() {
    const result = await request('setup_getconfig');
    if (result && result.success) {
        document.getElementById('system-name').value = result.data.systemName;
        document.getElementById('system-version').value = result.data.version;
        document.getElementById('maintenance-mode').checked = result.data.isMaintenance;
    }
}

async function saveConfig() {
    const data = {
        systemName: document.getElementById('system-name').value,
        version: document.getElementById('system-version').value,
        isMaintenance: document.getElementById('maintenance-mode').checked
    };

    const result = await request('setup_updateconfig', {
        method: 'POST',
        body: JSON.stringify(data)
    });

    if (result && result.success) {
        showToast('配置保存成功', 'success');
    } else {
        showToast(result?.message || '保存失败', 'error');
    }
}

async function checkHealth() {
    const result = await request('setup_healthcheck');
    const div = document.getElementById('health-result');
    if (result && result.success) {
        div.innerHTML = `<div class="success">✓ 系统正常 - ${result.data.time}</div>`;
    } else {
        div.innerHTML = `<div class="error">✗ 系统异常</div>`;
    }
}

function formatDate(dateStr) {
    const d = new Date(dateStr);
    return d.toLocaleString('zh-CN');
}

function formatDateTimeLocal(dateStr) {
    const d = new Date(dateStr);
    const pad = n => n.toString().padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function showToast(message, type = 'info') {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = `toast show ${type}`;
    setTimeout(() => toast.classList.remove('show'), 3000);
}
