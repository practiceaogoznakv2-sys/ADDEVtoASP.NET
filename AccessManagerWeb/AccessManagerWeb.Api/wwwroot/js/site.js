// Конфигурация и константы// Конфигурация// API client functions

const CONFIG = {

    API: {const API_BASE_URL = window.location.origin;class ApiClient {

        BASE_URL: window.location.origin,

        ENDPOINTS: {const AUTH_ENDPOINTS = {    async request(endpoint, options = {}) {

            AUTH: {

                USER_INFO: '/api/auth/userinfo',    USER_INFO: '/api/auth/userinfo',        const token = getAuthToken();

                LOGOUT: '/api/auth/logout'

            },    LOGOUT: '/api/auth/logout'        if (token) {

            REQUESTS: {

                LIST: '/api/resourcerequests',};            options.headers = {

                CREATE: '/api/resourcerequests',

                UPDATE_STATUS: (id) => `/api/resourcerequests/${id}/status`const REQUESTS_ENDPOINTS = {                ...options.headers,

            }

        }    LIST: '/api/resourcerequests',                'Authorization': `Bearer ${token}`

    },

    UI: {    CREATE: '/api/resourcerequests',            };

        ANIMATION_DURATION: 300,

        TOAST_TIMEOUT: 3000    UPDATE_STATUS: (id) => `/api/resourcerequests/${id}/status`        }

    }

};};



// Инициализация Toast уведомлений        const response = await fetch(`/api/${endpoint}`, options);

toastr.options = {

    closeButton: true,// Настройка toastr        if (!response.ok) {

    progressBar: true,

    positionClass: "toast-top-right",toastr.options = {            throw new Error(`API error: ${response.statusText}`);

    timeOut: CONFIG.UI.TOAST_TIMEOUT,

    showDuration: "300",    closeButton: true,        }

    hideDuration: "1000",

    extendedTimeOut: "1000",    progressBar: true,

    showEasing: "swing",

    hideEasing: "linear",    positionClass: "toast-top-right",        return response.json();

    showMethod: "fadeIn",

    hideMethod: "fadeOut"    timeOut: 3000    }

};

};

// API клиент

class ApiClient {    async getMyRequests() {

    static async fetch(endpoint, options = {}) {

        const defaultOptions = {// Утилиты для работы с API        return this.request('resourcerequests');

            credentials: 'include',

            headers: {async function fetchApi(endpoint, options = {}) {    }

                'Content-Type': 'application/json',

            }    const defaultOptions = {

        };

        credentials: 'include',    async getPendingRequests() {

        try {

            const response = await fetch(`${CONFIG.API.BASE_URL}${endpoint}`, {        headers: {        return this.request('resourcerequests/pending');

                ...defaultOptions,

                ...options            'Content-Type': 'application/json',    }

            });

        }

            if (!response.ok) {

                const error = await response.json().catch(() => ({    };    async createRequest(requestData) {

                    message: 'Произошла неизвестная ошибка'

                }));        return this.request('resourcerequests', {

                throw new Error(error.message || `HTTP error! status: ${response.status}`);

            }    try {            method: 'POST',



            return await response.json();        const response = await fetch(`${API_BASE_URL}${endpoint}`, { ...defaultOptions, ...options });            headers: {

        } catch (error) {

            console.error('API Error:', error);        if (!response.ok) {                'Content-Type': 'application/json'

            throw error;

        }            throw new Error(`HTTP error! status: ${response.status}`);            },

    }

        }            body: JSON.stringify(requestData)

    static async getUserInfo() {

        return await this.fetch(CONFIG.API.ENDPOINTS.AUTH.USER_INFO);        return await response.json();        });

    }

    } catch (error) {    }

    static async getRequests() {

        return await this.fetch(CONFIG.API.ENDPOINTS.REQUESTS.LIST);        console.error('API Error:', error);

    }

        toastr.error(error.message);    async approveRequest(id) {

    static async createRequest(data) {

        return await this.fetch(CONFIG.API.ENDPOINTS.REQUESTS.CREATE, {        throw error;        return this.request(`resourcerequests/${id}/approve`, {

            method: 'POST',

            body: JSON.stringify(data)    }            method: 'PUT'

        });

    }}        });



    static async updateRequestStatus(requestId, status) {    }

        return await this.fetch(CONFIG.API.ENDPOINTS.REQUESTS.UPDATE_STATUS(requestId), {

            method: 'PUT',// Функции для работы с пользователем

            body: JSON.stringify({ status })

        });async function loadUserInfo() {    async denyRequest(id) {

    }

}    try {        return this.request(`resourcerequests/${id}/deny`, {



// UI компоненты        const userInfo = await fetchApi(AUTH_ENDPOINTS.USER_INFO);            method: 'PUT'

class UI {

    static getStatusBadge(status) {        displayUserInfo(userInfo);        });

        const statusConfig = {

            'Pending': { class: 'status-pending', icon: 'bi-hourglass-split', text: 'Ожидает' },    } catch (error) {    }

            'Approved': { class: 'status-approved', icon: 'bi-check-circle', text: 'Одобрено' },

            'Rejected': { class: 'status-rejected', icon: 'bi-x-circle', text: 'Отклонено' }        console.error('Error loading user info:', error);}

        };

    }

        const config = statusConfig[status] || { class: '', icon: 'bi-question-circle', text: status };

        }const api = new ApiClient();

        return `

            <span class="status-badge ${config.class}">

                <i class="bi ${config.icon}"></i>

                ${config.text}function displayUserInfo(userInfo) {// UI helper functions

            </span>

        `;    const userInfoElement = document.getElementById('userInfo');function showLoading() {

    }

    if (userInfoElement && userInfo) {    document.querySelector('.spinner').style.display = 'block';

    static getActionButtons(request) {

        if (request.status !== 'Pending') return '';        userInfoElement.innerHTML = `}



        return `            <i class="bi bi-person-circle"></i>

            <div class="btn-group" role="group">

                <button class="btn btn-sm btn-success btn-icon approve-btn"             ${userInfo.displayName || userInfo.username}function hideLoading() {

                        data-id="${request.id}" 

                        data-bs-toggle="tooltip"         `;    document.querySelector('.spinner').style.display = 'none';

                        title="Одобрить">

                    <i class="bi bi-check-lg"></i>    }}

                </button>

                <button class="btn btn-sm btn-danger btn-icon reject-btn" }

                        data-id="${request.id}" 

                        data-bs-toggle="tooltip" function showError(message) {

                        title="Отклонить">

                    <i class="bi bi-x-lg"></i>// Функции для работы с заявками    const errorElement = document.querySelector('.error-message');

                </button>

            </div>async function loadDashboardStats() {    errorElement.textContent = message;

        `;

    }    try {    errorElement.style.display = 'block';



    static displayUserInfo(userInfo) {        const requests = await fetchApi(REQUESTS_ENDPOINTS.LIST);}

        const userInfoElement = document.getElementById('userInfo');

        if (!userInfoElement || !userInfo) return;        const stats = calculateStats(requests);



        userInfoElement.innerHTML = `        displayDashboardStats(stats);function hideError() {

            <div class="d-flex align-items-center">

                <i class="bi bi-person-circle fs-4 me-2"></i>    } catch (error) {    document.querySelector('.error-message').style.display = 'none';

                <div>

                    <div class="fw-bold">${userInfo.displayName || userInfo.username}</div>        console.error('Error loading dashboard stats:', error);}

                    <small class="text-light">${userInfo.department || ''}</small>

                </div>    }

            </div>

        `;}// Request list rendering

    }

function renderRequestList(requests, showActions = false) {

    static displayDashboardStats(stats) {

        const statsContainer = document.getElementById('dashboardStats');function calculateStats(requests) {    return requests.map(request => `

        if (!statsContainer) return;

    return requests.reduce((acc, request) => {        <div class="card request-card">

        const cards = [

            {        acc[request.status] = (acc[request.status] || 0) + 1;            <div class="card-header d-flex justify-content-between align-items-center">

                status: 'Pending',

                icon: 'bi-hourglass-split',        return acc;                <h5 class="mb-0">${request.resourceName}</h5>

                title: 'Ожидающие',

                class: 'stat-pending'    }, {});                <span class="badge badge-${request.status.toLowerCase()}">${request.status}</span>

            },

            {}            </div>

                status: 'Approved',

                icon: 'bi-check-circle',            <div class="card-body">

                title: 'Одобренные',

                class: 'stat-approved'function displayDashboardStats(stats) {                <p class="card-text">${request.requestReason}</p>

            },

            {    const statsContainer = document.getElementById('dashboardStats');                <div class="text-muted small">

                status: 'Rejected',

                icon: 'bi-x-circle',    if (statsContainer) {                    Requested by: ${request.requestorUsername}<br>

                title: 'Отклоненные',

                class: 'stat-rejected'        statsContainer.innerHTML = `                    Date: ${new Date(request.requestDate).toLocaleDateString()}

            }

        ];            <div class="col-md-4">                </div>



        statsContainer.innerHTML = cards.map(card => `                <div class="dashboard-stat stat-pending">                ${showActions && request.status === 'Pending' ? `

            <div class="col-md-4">

                <div class="dashboard-stat ${card.class}">                    <h3>${stats.Pending || 0}</h3>                    <div class="mt-3">

                    <i class="bi ${card.icon} stat-icon"></i>

                    <h3>${stats[card.status] || 0}</h3>                    <p>Ожидающие</p>                        <button class="btn btn-success btn-sm" onclick="approveRequest(${request.id})">

                    <p>${card.title}</p>

                </div>                </div>                            Approve

            </div>

        `).join('');            </div>                        </button>

    }

            <div class="col-md-4">                        <button class="btn btn-danger btn-sm" onclick="denyRequest(${request.id})">

    static displayRequests(requests) {

        const tableBody = document.getElementById('requestsTable');                <div class="dashboard-stat stat-approved">                            Deny

        if (!tableBody) return;

                    <h3>${stats.Approved || 0}</h3>                        </button>

        if (!requests.length) {

            tableBody.innerHTML = `                    <p>Одобренные</p>                    </div>

                <tr>

                    <td colspan="6" class="text-center py-4">                </div>                ` : ''}

                        <i class="bi bi-inbox display-4 d-block mb-2 text-muted"></i>

                        <p class="text-muted">Нет активных заявок</p>            </div>            </div>

                    </td>

                </tr>            <div class="col-md-4">        </div>

            `;

            return;                <div class="dashboard-stat stat-rejected">    `).join('');

        }

                    <h3>${stats.Rejected || 0}</h3>}

        tableBody.innerHTML = requests.map(request => `

            <tr class="align-middle">                    <p>Отклоненные</p>

                <td>#${request.id}</td>

                <td>                </div>// Event handlers

                    <div class="fw-bold">${request.resourceName}</div>

                    <small class="text-muted">${request.description || ''}</small>            </div>async function loadMyRequests() {

                </td>

                <td>        `;    try {

                    <div class="d-flex align-items-center">

                        <i class="bi bi-person-circle me-2"></i>    }        showLoading();

                        ${request.requestorUsername}

                    </div>}        hideError();

                </td>

                <td>        const requests = await api.getMyRequests();

                    <div>${new Date(request.createdAt).toLocaleDateString()}</div>

                    <small class="text-muted">async function loadRequests() {        document.getElementById('mainContent').innerHTML = `

                        ${new Date(request.createdAt).toLocaleTimeString()}

                    </small>    try {            <div class="form-container">

                </td>

                <td>${this.getStatusBadge(request.status)}</td>        const requests = await fetchApi(REQUESTS_ENDPOINTS.LIST);                <h2>My Access Requests</h2>

                <td>${this.getActionButtons(request)}</td>

            </tr>        displayRequests(requests);                <button class="btn btn-primary mb-4" onclick="showNewRequestForm()">

        `).join('');

    } catch (error) {                    New Request

        // Инициализация тултипов

        const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');        console.error('Error loading requests:', error);                </button>

        tooltips.forEach(tooltip => new bootstrap.Tooltip(tooltip));

    }    }                <div id="requestsList">



    static showLoader() {}                    ${renderRequestList(requests)}

        // Можно добавить анимацию загрузки

        document.body.style.cursor = 'wait';                </div>

    }

function displayRequests(requests) {            </div>

    static hideLoader() {

        document.body.style.cursor = 'default';    const tableBody = document.getElementById('requestsTable');        `;

    }

}    if (!tableBody) return;    } catch (error) {



// Основной класс приложения        showError(error.message);

class App {

    static async init() {    tableBody.innerHTML = requests.map(request => `    } finally {

        this.attachEventListeners();

        await this.loadInitialData();        <tr>        hideLoading();

    }

            <td>${request.id}</td>    }

    static attachEventListeners() {

        // Обработчик формы создания заявки            <td>${request.resourceName}</td>}

        const submitButton = document.getElementById('submitRequest');

        if (submitButton) {            <td>${request.requestorUsername}</td>

            submitButton.onclick = this.handleRequestSubmit.bind(this);

        }            <td>${new Date(request.createdAt).toLocaleDateString()}</td>async function loadPendingApprovals() {



        // Обработчики кнопок действий            <td>${getStatusBadge(request.status)}</td>    try {

        document.addEventListener('click', async (e) => {

            if (e.target.closest('.approve-btn')) {            <td>${getActionButtons(request)}</td>        showLoading();

                const requestId = e.target.closest('.approve-btn').dataset.id;

                await this.handleStatusUpdate(requestId, 'Approved');        </tr>        hideError();

            }

            else if (e.target.closest('.reject-btn')) {    `).join('');        const requests = await api.getPendingRequests();

                const requestId = e.target.closest('.reject-btn').dataset.id;

                await this.handleStatusUpdate(requestId, 'Rejected');        document.getElementById('mainContent').innerHTML = `

            }

        });    attachActionHandlers();            <div class="form-container">



        // Сброс формы при закрытии модального окна}                <h2>Pending Approvals</h2>

        const newRequestModal = document.getElementById('newRequestModal');

        if (newRequestModal) {                <div id="approvalsList">

            newRequestModal.addEventListener('hidden.bs.modal', () => {

                document.getElementById('requestForm').reset();function getStatusBadge(status) {                    ${renderRequestList(requests, true)}

            });

        }    const statusClasses = {                </div>

    }

        'Pending': 'status-pending',            </div>

    static async loadInitialData() {

        UI.showLoader();        'Approved': 'status-approved',        `;

        try {

            const [userInfo, requests] = await Promise.all([        'Rejected': 'status-rejected'    } catch (error) {

                ApiClient.getUserInfo(),

                ApiClient.getRequests()    };        showError(error.message);

            ]);

    return `<span class="status-badge ${statusClasses[status] || ''}">${status}</span>`;    } finally {

            UI.displayUserInfo(userInfo);

            UI.displayRequests(requests);}        hideLoading();

            UI.displayDashboardStats(this.calculateStats(requests));

        } catch (error) {    }

            toastr.error('Ошибка при загрузке данных');

            console.error('Error loading initial data:', error);function getActionButtons(request) {}

        } finally {

            UI.hideLoader();    if (request.status === 'Pending') {

        }

    }        return `function showNewRequestForm() {



    static calculateStats(requests) {            <button class="btn btn-sm btn-success btn-icon approve-btn" data-id="${request.id}">    document.getElementById('mainContent').innerHTML = `

        return requests.reduce((acc, request) => {

            acc[request.status] = (acc[request.status] || 0) + 1;                <i class="bi bi-check-lg"></i>        <div class="form-container">

            return acc;

        }, {});            </button>            <h2>New Access Request</h2>

    }

            <button class="btn btn-sm btn-danger btn-icon reject-btn" data-id="${request.id}">            <form id="requestForm" onsubmit="submitRequest(event)">

    static async handleRequestSubmit(event) {

        event.preventDefault();                <i class="bi bi-x-lg"></i>                <div class="mb-3">

        

        const requestData = {            </button>                    <label for="resourceName" class="form-label">Resource Name</label>

            resourceName: document.getElementById('resource').value,

            description: document.getElementById('description').value        `;                    <input type="text" class="form-control" id="resourceName" required>

        };

    }                </div>

        if (!requestData.resourceName) {

            toastr.warning('Укажите название ресурса');    return '';                <div class="mb-3">

            return;

        }}                    <label for="requestReason" class="form-label">Reason for Request</label>



        UI.showLoader();                    <textarea class="form-control" id="requestReason" rows="3" required></textarea>

        try {

            await ApiClient.createRequest(requestData);async function updateRequestStatus(requestId, status) {                </div>

            toastr.success('Заявка успешно создана');

                try {                <button type="submit" class="btn btn-primary">Submit Request</button>

            const modal = bootstrap.Modal.getInstance(document.getElementById('newRequestModal'));

            modal.hide();        await fetchApi(REQUESTS_ENDPOINTS.UPDATE_STATUS(requestId), {                <button type="button" class="btn btn-secondary" onclick="loadMyRequests()">Cancel</button>

            

            document.getElementById('requestForm').reset();            method: 'PUT',            </form>

            await this.loadInitialData();

        } catch (error) {            body: JSON.stringify({ status })        </div>

            toastr.error('Ошибка при создании заявки');

            console.error('Error creating request:', error);        });    `;

        } finally {

            UI.hideLoader();        toastr.success('Статус заявки обновлен');}

        }

    }        await Promise.all([loadRequests(), loadDashboardStats()]);



    static async handleStatusUpdate(requestId, status) {    } catch (error) {async function submitRequest(event) {

        UI.showLoader();

        try {        console.error('Error updating request status:', error);    event.preventDefault();

            await ApiClient.updateRequestStatus(requestId, status);

            toastr.success('Статус заявки обновлен');    }    try {

            await this.loadInitialData();

        } catch (error) {}        showLoading();

            toastr.error('Ошибка при обновлении статуса');

            console.error('Error updating status:', error);        hideError();

        } finally {

            UI.hideLoader();function attachActionHandlers() {        

        }

    }    document.querySelectorAll('.approve-btn').forEach(btn => {        const requestData = {

}

        btn.onclick = () => updateRequestStatus(btn.dataset.id, 'Approved');            resourceName: document.getElementById('resourceName').value,

// Инициализация приложения

document.addEventListener('DOMContentLoaded', () => App.init());    });            requestReason: document.getElementById('requestReason').value

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