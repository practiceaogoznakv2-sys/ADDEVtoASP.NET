// Authentication functions
const TOKEN_KEY = 'access_token';
const USER_KEY = 'user_profile';

async function login(username, password) {
    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        });

        if (!response.ok) {
            throw new Error('Login failed');
        }

        const data = await response.json();
        localStorage.setItem(TOKEN_KEY, data.token);
        localStorage.setItem(USER_KEY, JSON.stringify(data.userProfile));
        updateUIForAuthenticatedUser(data.userProfile);
        return data;
    } catch (error) {
        console.error('Login error:', error);
        throw error;
    }
}

function logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    updateUIForAnonymousUser();
    window.location.href = '/login';
}

function getAuthToken() {
    return localStorage.getItem(TOKEN_KEY);
}

function getUserProfile() {
    const profile = localStorage.getItem(USER_KEY);
    return profile ? JSON.parse(profile) : null;
}

function isAuthenticated() {
    return !!getAuthToken();
}

function updateUIForAuthenticatedUser(userProfile) {
    document.getElementById('loginNav').style.display = 'none';
    document.getElementById('userNav').style.display = 'block';
    document.getElementById('userDisplay').textContent = userProfile.displayName || userProfile.username;
    
    // Show/hide approver-specific elements
    const approverElements = document.querySelectorAll('.approver-only');
    const isApprover = userProfile.roles?.includes('Approver');
    approverElements.forEach(el => {
        el.style.display = isApprover ? '' : 'none';
    });
}

function updateUIForAnonymousUser() {
    document.getElementById('loginNav').style.display = 'block';
    document.getElementById('userNav').style.display = 'none';
    document.getElementById('userDisplay').textContent = '';
    
    const approverElements = document.querySelectorAll('.approver-only');
    approverElements.forEach(el => {
        el.style.display = 'none';
    });
}

// Initialize UI based on authentication state
document.addEventListener('DOMContentLoaded', () => {
    const userProfile = getUserProfile();
    if (userProfile) {
        updateUIForAuthenticatedUser(userProfile);
    } else {
        updateUIForAnonymousUser();
    }
});