// API client functions
class ApiClient {
    async request(endpoint, options = {}) {
        const token = getAuthToken();
        if (token) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        }

        const response = await fetch(`/api/${endpoint}`, options);
        if (!response.ok) {
            throw new Error(`API error: ${response.statusText}`);
        }

        return response.json();
    }

    async getMyRequests() {
        return this.request('resourcerequests');
    }

    async getPendingRequests() {
        return this.request('resourcerequests/pending');
    }

    async createRequest(requestData) {
        return this.request('resourcerequests', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });
    }

    async approveRequest(id) {
        return this.request(`resourcerequests/${id}/approve`, {
            method: 'PUT'
        });
    }

    async denyRequest(id) {
        return this.request(`resourcerequests/${id}/deny`, {
            method: 'PUT'
        });
    }
}

const api = new ApiClient();

// UI helper functions
function showLoading() {
    document.querySelector('.spinner').style.display = 'block';
}

function hideLoading() {
    document.querySelector('.spinner').style.display = 'none';
}

function showError(message) {
    const errorElement = document.querySelector('.error-message');
    errorElement.textContent = message;
    errorElement.style.display = 'block';
}

function hideError() {
    document.querySelector('.error-message').style.display = 'none';
}

// Request list rendering
function renderRequestList(requests, showActions = false) {
    return requests.map(request => `
        <div class="card request-card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">${request.resourceName}</h5>
                <span class="badge badge-${request.status.toLowerCase()}">${request.status}</span>
            </div>
            <div class="card-body">
                <p class="card-text">${request.requestReason}</p>
                <div class="text-muted small">
                    Requested by: ${request.requestorUsername}<br>
                    Date: ${new Date(request.requestDate).toLocaleDateString()}
                </div>
                ${showActions && request.status === 'Pending' ? `
                    <div class="mt-3">
                        <button class="btn btn-success btn-sm" onclick="approveRequest(${request.id})">
                            Approve
                        </button>
                        <button class="btn btn-danger btn-sm" onclick="denyRequest(${request.id})">
                            Deny
                        </button>
                    </div>
                ` : ''}
            </div>
        </div>
    `).join('');
}

// Event handlers
async function loadMyRequests() {
    try {
        showLoading();
        hideError();
        const requests = await api.getMyRequests();
        document.getElementById('mainContent').innerHTML = `
            <div class="form-container">
                <h2>My Access Requests</h2>
                <button class="btn btn-primary mb-4" onclick="showNewRequestForm()">
                    New Request
                </button>
                <div id="requestsList">
                    ${renderRequestList(requests)}
                </div>
            </div>
        `;
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

async function loadPendingApprovals() {
    try {
        showLoading();
        hideError();
        const requests = await api.getPendingRequests();
        document.getElementById('mainContent').innerHTML = `
            <div class="form-container">
                <h2>Pending Approvals</h2>
                <div id="approvalsList">
                    ${renderRequestList(requests, true)}
                </div>
            </div>
        `;
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

function showNewRequestForm() {
    document.getElementById('mainContent').innerHTML = `
        <div class="form-container">
            <h2>New Access Request</h2>
            <form id="requestForm" onsubmit="submitRequest(event)">
                <div class="mb-3">
                    <label for="resourceName" class="form-label">Resource Name</label>
                    <input type="text" class="form-control" id="resourceName" required>
                </div>
                <div class="mb-3">
                    <label for="requestReason" class="form-label">Reason for Request</label>
                    <textarea class="form-control" id="requestReason" rows="3" required></textarea>
                </div>
                <button type="submit" class="btn btn-primary">Submit Request</button>
                <button type="button" class="btn btn-secondary" onclick="loadMyRequests()">Cancel</button>
            </form>
        </div>
    `;
}

async function submitRequest(event) {
    event.preventDefault();
    try {
        showLoading();
        hideError();
        
        const requestData = {
            resourceName: document.getElementById('resourceName').value,
            requestReason: document.getElementById('requestReason').value
        };

        await api.createRequest(requestData);
        await loadMyRequests();
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

async function approveRequest(id) {
    try {
        showLoading();
        hideError();
        await api.approveRequest(id);
        await loadPendingApprovals();
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

async function denyRequest(id) {
    try {
        showLoading();
        hideError();
        await api.denyRequest(id);
        await loadPendingApprovals();
    } catch (error) {
        showError(error.message);
    } finally {
        hideLoading();
    }
}

// Route handling
function handleRoute() {
    const path = window.location.pathname;
    if (!isAuthenticated() && path !== '/login') {
        window.location.href = '/login';
        return;
    }

    switch (path) {
        case '/requests':
            loadMyRequests();
            break;
        case '/approvals':
            loadPendingApprovals();
            break;
        case '/login':
            if (isAuthenticated()) {
                window.location.href = '/requests';
            }
            break;
    }
}

// Initialize
window.addEventListener('popstate', handleRoute);
document.addEventListener('DOMContentLoaded', handleRoute);