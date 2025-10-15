// Конфигурация// API client functions

const API_BASE_URL = window.location.origin;class ApiClient {

const AUTH_ENDPOINTS = {    async request(endpoint, options = {}) {

    USER_INFO: '/api/auth/userinfo',        const token = getAuthToken();

    LOGOUT: '/api/auth/logout'        if (token) {

};            options.headers = {

const REQUESTS_ENDPOINTS = {                ...options.headers,

    LIST: '/api/resourcerequests',                'Authorization': `Bearer ${token}`

    CREATE: '/api/resourcerequests',            };

    UPDATE_STATUS: (id) => `/api/resourcerequests/${id}/status`        }

};

        const response = await fetch(`/api/${endpoint}`, options);

// Настройка toastr        if (!response.ok) {

toastr.options = {            throw new Error(`API error: ${response.statusText}`);

    closeButton: true,        }

    progressBar: true,

    positionClass: "toast-top-right",        return response.json();

    timeOut: 3000    }

};

    async getMyRequests() {

// Утилиты для работы с API        return this.request('resourcerequests');

async function fetchApi(endpoint, options = {}) {    }

    const defaultOptions = {

        credentials: 'include',    async getPendingRequests() {

        headers: {        return this.request('resourcerequests/pending');

            'Content-Type': 'application/json',    }

        }

    };    async createRequest(requestData) {

        return this.request('resourcerequests', {

    try {            method: 'POST',

        const response = await fetch(`${API_BASE_URL}${endpoint}`, { ...defaultOptions, ...options });            headers: {

        if (!response.ok) {                'Content-Type': 'application/json'

            throw new Error(`HTTP error! status: ${response.status}`);            },

        }            body: JSON.stringify(requestData)

        return await response.json();        });

    } catch (error) {    }

        console.error('API Error:', error);

        toastr.error(error.message);    async approveRequest(id) {

        throw error;        return this.request(`resourcerequests/${id}/approve`, {

    }            method: 'PUT'

}        });

    }

// Функции для работы с пользователем

async function loadUserInfo() {    async denyRequest(id) {

    try {        return this.request(`resourcerequests/${id}/deny`, {

        const userInfo = await fetchApi(AUTH_ENDPOINTS.USER_INFO);            method: 'PUT'

        displayUserInfo(userInfo);        });

    } catch (error) {    }

        console.error('Error loading user info:', error);}

    }

}const api = new ApiClient();



function displayUserInfo(userInfo) {// UI helper functions

    const userInfoElement = document.getElementById('userInfo');function showLoading() {

    if (userInfoElement && userInfo) {    document.querySelector('.spinner').style.display = 'block';

        userInfoElement.innerHTML = `}

            <i class="bi bi-person-circle"></i>

            ${userInfo.displayName || userInfo.username}function hideLoading() {

        `;    document.querySelector('.spinner').style.display = 'none';

    }}

}

function showError(message) {

// Функции для работы с заявками    const errorElement = document.querySelector('.error-message');

async function loadDashboardStats() {    errorElement.textContent = message;

    try {    errorElement.style.display = 'block';

        const requests = await fetchApi(REQUESTS_ENDPOINTS.LIST);}

        const stats = calculateStats(requests);

        displayDashboardStats(stats);function hideError() {

    } catch (error) {    document.querySelector('.error-message').style.display = 'none';

        console.error('Error loading dashboard stats:', error);}

    }

}// Request list rendering

function renderRequestList(requests, showActions = false) {

function calculateStats(requests) {    return requests.map(request => `

    return requests.reduce((acc, request) => {        <div class="card request-card">

        acc[request.status] = (acc[request.status] || 0) + 1;            <div class="card-header d-flex justify-content-between align-items-center">

        return acc;                <h5 class="mb-0">${request.resourceName}</h5>

    }, {});                <span class="badge badge-${request.status.toLowerCase()}">${request.status}</span>

}            </div>

            <div class="card-body">

function displayDashboardStats(stats) {                <p class="card-text">${request.requestReason}</p>

    const statsContainer = document.getElementById('dashboardStats');                <div class="text-muted small">

    if (statsContainer) {                    Requested by: ${request.requestorUsername}<br>

        statsContainer.innerHTML = `                    Date: ${new Date(request.requestDate).toLocaleDateString()}

            <div class="col-md-4">                </div>

                <div class="dashboard-stat stat-pending">                ${showActions && request.status === 'Pending' ? `

                    <h3>${stats.Pending || 0}</h3>                    <div class="mt-3">

                    <p>Ожидающие</p>                        <button class="btn btn-success btn-sm" onclick="approveRequest(${request.id})">

                </div>                            Approve

            </div>                        </button>

            <div class="col-md-4">                        <button class="btn btn-danger btn-sm" onclick="denyRequest(${request.id})">

                <div class="dashboard-stat stat-approved">                            Deny

                    <h3>${stats.Approved || 0}</h3>                        </button>

                    <p>Одобренные</p>                    </div>

                </div>                ` : ''}

            </div>            </div>

            <div class="col-md-4">        </div>

                <div class="dashboard-stat stat-rejected">    `).join('');

                    <h3>${stats.Rejected || 0}</h3>}

                    <p>Отклоненные</p>

                </div>// Event handlers

            </div>async function loadMyRequests() {

        `;    try {

    }        showLoading();

}        hideError();

        const requests = await api.getMyRequests();

async function loadRequests() {        document.getElementById('mainContent').innerHTML = `

    try {            <div class="form-container">

        const requests = await fetchApi(REQUESTS_ENDPOINTS.LIST);                <h2>My Access Requests</h2>

        displayRequests(requests);                <button class="btn btn-primary mb-4" onclick="showNewRequestForm()">

    } catch (error) {                    New Request

        console.error('Error loading requests:', error);                </button>

    }                <div id="requestsList">

}                    ${renderRequestList(requests)}

                </div>

function displayRequests(requests) {            </div>

    const tableBody = document.getElementById('requestsTable');        `;

    if (!tableBody) return;    } catch (error) {

        showError(error.message);

    tableBody.innerHTML = requests.map(request => `    } finally {

        <tr>        hideLoading();

            <td>${request.id}</td>    }

            <td>${request.resourceName}</td>}

            <td>${request.requestorUsername}</td>

            <td>${new Date(request.createdAt).toLocaleDateString()}</td>async function loadPendingApprovals() {

            <td>${getStatusBadge(request.status)}</td>    try {

            <td>${getActionButtons(request)}</td>        showLoading();

        </tr>        hideError();

    `).join('');        const requests = await api.getPendingRequests();

        document.getElementById('mainContent').innerHTML = `

    attachActionHandlers();            <div class="form-container">

}                <h2>Pending Approvals</h2>

                <div id="approvalsList">

function getStatusBadge(status) {                    ${renderRequestList(requests, true)}

    const statusClasses = {                </div>

        'Pending': 'status-pending',            </div>

        'Approved': 'status-approved',        `;

        'Rejected': 'status-rejected'    } catch (error) {

    };        showError(error.message);

    return `<span class="status-badge ${statusClasses[status] || ''}">${status}</span>`;    } finally {

}        hideLoading();

    }

function getActionButtons(request) {}

    if (request.status === 'Pending') {

        return `function showNewRequestForm() {

            <button class="btn btn-sm btn-success btn-icon approve-btn" data-id="${request.id}">    document.getElementById('mainContent').innerHTML = `

                <i class="bi bi-check-lg"></i>        <div class="form-container">

            </button>            <h2>New Access Request</h2>

            <button class="btn btn-sm btn-danger btn-icon reject-btn" data-id="${request.id}">            <form id="requestForm" onsubmit="submitRequest(event)">

                <i class="bi bi-x-lg"></i>                <div class="mb-3">

            </button>                    <label for="resourceName" class="form-label">Resource Name</label>

        `;                    <input type="text" class="form-control" id="resourceName" required>

    }                </div>

    return '';                <div class="mb-3">

}                    <label for="requestReason" class="form-label">Reason for Request</label>

                    <textarea class="form-control" id="requestReason" rows="3" required></textarea>

async function updateRequestStatus(requestId, status) {                </div>

    try {                <button type="submit" class="btn btn-primary">Submit Request</button>

        await fetchApi(REQUESTS_ENDPOINTS.UPDATE_STATUS(requestId), {                <button type="button" class="btn btn-secondary" onclick="loadMyRequests()">Cancel</button>

            method: 'PUT',            </form>

            body: JSON.stringify({ status })        </div>

        });    `;

        toastr.success('Статус заявки обновлен');}

        await Promise.all([loadRequests(), loadDashboardStats()]);

    } catch (error) {async function submitRequest(event) {

        console.error('Error updating request status:', error);    event.preventDefault();

    }    try {

}        showLoading();

        hideError();

function attachActionHandlers() {        

    document.querySelectorAll('.approve-btn').forEach(btn => {        const requestData = {

        btn.onclick = () => updateRequestStatus(btn.dataset.id, 'Approved');            resourceName: document.getElementById('resourceName').value,

    });            requestReason: document.getElementById('requestReason').value

        };

    document.querySelectorAll('.reject-btn').forEach(btn => {

        btn.onclick = () => updateRequestStatus(btn.dataset.id, 'Rejected');        await api.createRequest(requestData);

    });        await loadMyRequests();

}    } catch (error) {

        showError(error.message);

// Обработка формы создания заявки    } finally {

async function handleRequestSubmit(event) {        hideLoading();

    event.preventDefault();    }

    }

    const requestData = {

        resourceName: document.getElementById('resource').value,async function approveRequest(id) {

        description: document.getElementById('description').value    try {

    };        showLoading();

        hideError();

    try {        await api.approveRequest(id);

        await fetchApi(REQUESTS_ENDPOINTS.CREATE, {        await loadPendingApprovals();

            method: 'POST',    } catch (error) {

            body: JSON.stringify(requestData)        showError(error.message);

        });    } finally {

                hideLoading();

        toastr.success('Заявка успешно создана');    }

        $('#newRequestModal').modal('hide');}

        document.getElementById('requestForm').reset();

        async function denyRequest(id) {

        await Promise.all([loadRequests(), loadDashboardStats()]);    try {

    } catch (error) {        showLoading();

        console.error('Error creating request:', error);        hideError();

    }        await api.denyRequest(id);

}        await loadPendingApprovals();

    } catch (error) {

// Инициализация        showError(error.message);

document.addEventListener('DOMContentLoaded', () => {    } finally {

    // Загрузка начальных данных        hideLoading();

    Promise.all([    }

        loadUserInfo(),}

        loadDashboardStats(),

        loadRequests()// Route handling

    ]);function handleRoute() {

    const path = window.location.pathname;

    // Привязка обработчиков событий    if (!isAuthenticated() && path !== '/login') {

    const submitButton = document.getElementById('submitRequest');        window.location.href = '/login';

    if (submitButton) {        return;

        submitButton.onclick = handleRequestSubmit;    }

    }

    switch (path) {

    // Сброс формы при закрытии модального окна        case '/requests':

    const newRequestModal = document.getElementById('newRequestModal');            loadMyRequests();

    if (newRequestModal) {            break;

        newRequestModal.addEventListener('hidden.bs.modal', () => {        case '/approvals':

            document.getElementById('requestForm').reset();            loadPendingApprovals();

        });            break;

    }        case '/login':

});            if (isAuthenticated()) {
                window.location.href = '/requests';
            }
            break;
    }
}

// Initialize
window.addEventListener('popstate', handleRoute);
document.addEventListener('DOMContentLoaded', handleRoute);