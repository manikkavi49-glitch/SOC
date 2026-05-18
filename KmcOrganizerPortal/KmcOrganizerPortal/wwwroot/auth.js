const API_BASE = "https://localhost:7108/api";

// --- 1. LOGIN LOGIC ---
document.getElementById('loginForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPass').value;

    try {
        const response = await fetch(`${API_BASE}/Organizers/Login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email: email, password: password })
        });
        const result = await response.json();
        if (response.ok) {
            localStorage.setItem('isLoggedIn', 'true');
            localStorage.setItem('userEmail', email);
            localStorage.setItem('userName', result.userName);
            window.location.href = "dashboard.html";
        } else {
            alert("Error: " + result.message);
        }
    } catch (error) {
        console.error("Login Error:", error);
        alert("API එකට සම්බන්ධ වීමට නොහැක.");
    }
});

// --- 2. REGISTER LOGIC ---
document.getElementById('registerForm')?.addEventListener('submit', async (e) => {
    e.preventDefault();
    const name = document.getElementById('regName').value;
    const email = document.getElementById('regEmail').value;
    const password = document.getElementById('regPass').value;

    try {
        const response = await fetch(`${API_BASE}/Organizers/Register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: name, email: email, password: password })
        });
        if (response.ok) {
            alert("Registration Successful! Please Login.");
            window.location.href = "login.html";
        } else {
            alert("Registration Failed!");
        }
    } catch (error) {
        alert("Error connecting to API");
    }
});

// --- 3. DASHBOARD NAVIGATION & CONTENT ---
function showSection(section) {
    const title = document.getElementById('page-title');
    const display = document.getElementById('main-display-area');

    document.querySelectorAll('.sidebar nav a').forEach(el => el.classList.remove('active'));
    document.getElementById('nav-' + section)?.classList.add('active');

    if (section === 'dashboard') {
        title.innerText = "Dashboard Overview";
        display.innerHTML = `<div id="stats-area">Loading Statistics...</div>`;
        loadDashboardStats();
    }
    else if (section === 'events') {
        title.innerText = "My Managed Events";
        display.innerHTML = `
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                <p style="color: var(--text-sec); margin:0;">Manage and monitor your scheduled events.</p>
                <button onclick="openModal()" class="btn-add" style="background:var(--accent); color:white; border:none; padding:10px 20px; border-radius:10px; cursor:pointer; font-weight:600;">
                    <i class="bi bi-plus-lg"></i> Add New Event
                </button>
            </div>
            <div id="events-table-div" class="card">Loading events...</div>`;
        loadOrganizerEvents();
    }
    else if (section === 'privacy') {
        title.innerText = "Privacy Policy";
        display.innerHTML = `
            <div class="card">
                <h3><i class="bi bi-shield-lock"></i> Security & Privacy</h3>
                <p>As part of our SOA implementation, we ensure:</p>
                <ul>
                    <li>Data Isolation: Organizers only see their own events.</li>
                    <li>Secure Endpoints: All API requests are authenticated.</li>
                    <li>Daily Backups: Database redundancy is maintained.</li>
                </ul>
            </div>`;
    }
}

// Modal functions
function openModal() {
    document.querySelector('#eventModal h2').innerText = "Create New Event";
    document.querySelector('#eventModal .btn-login').innerHTML = '<i class="bi bi-cloud-upload"></i> Publish Event';

    document.getElementById('addEventForm').reset();
    // Default to Public when opening a new form
    document.getElementById('eventVisibility').value = 'Public';
    if (document.getElementById('currentEventId')) document.getElementById('currentEventId').value = "";

    document.getElementById('eventModal').style.display = "block";
}

function closeModal() {
    document.getElementById('eventModal').style.display = "none";
}

function closeViewModal() {
    document.getElementById('viewModal').style.display = "none";
}

// --- 4. DATA OPERATIONS (Add/Update/Delete/View) ---

// Create OR Update Event (PUT/POST)
async function handleAddEvent(e) {
    e.preventDefault();

    const id = document.getElementById('currentEventId')?.value;

    const eventData = {
        title: document.getElementById('eventTitle').value,
        type: document.getElementById('eventType').value,
        location: document.getElementById('eventLocation').value,
        description: document.getElementById('eventDesc').value,
        // Visibility data එකතු කළා
        visibility: document.getElementById('eventVisibility').value,
        eventDate: document.getElementById('eventDate').value + "T" + document.getElementById('eventTime').value,
        organizerEmail: localStorage.getItem('userEmail')
    };

    if (id) eventData.id = parseInt(id);

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_BASE}/Events/${id}` : `${API_BASE}/Events`;

    try {
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(eventData)
        });

        if (response.ok) {
            alert(id ? "Event Updated Successfully!" : "Event Added Successfully!");
            closeModal();
            showSection('events');
        } else {
            const errorData = await response.text();
            alert("Error saving event: " + errorData);
        }
    } catch (e) { console.error(e); }
}

async function loadOrganizerEvents() {
    const email = localStorage.getItem('userEmail');
    try {
        const response = await fetch(`${API_BASE}/Events/ByOrganizer/${email}`);
        const data = await response.json();

        let html = `
            <table style="width:100%; border-collapse: collapse;">
                <thead>
                    <tr style="text-align:left; border-bottom: 1px solid #f4f7fe;">
                        <th style="padding:15px; color:var(--text-sec);">Event Title</th>
                        <th style="padding:15px; color:var(--text-sec);">Visibility</th>
                        <th style="padding:15px; color:var(--text-sec);">Location</th>
                        <th style="padding:15px; color:var(--text-sec);">Date & Time</th>
                        <th style="padding:15px; color:var(--text-sec);">Action</th>
                    </tr>
                </thead>
                <tbody>`;

        data.forEach(ev => {
            // Visibility එක අනුව වර්ණය වෙනස් කිරීම
            const visBadgeBg = ev.visibility === 'Private' ? '#f1f5f9' : '#dcfce3';
            const visBadgeColor = ev.visibility === 'Private' ? '#64748b' : '#16a34a';
            const visText = ev.visibility === 'Private' ? '🔒 Private' : '🌍 Public';

            html += `
        <tr style="border-bottom: 1px solid #f4f7fe;">
            <td style="padding:15px;"><b>${ev.title}</b><br><small style="color:#a3aed0;">${ev.description || ''}</small></td>
            <td style="padding:15px;"><span style="padding:5px 10px; border-radius:8px; font-size:12px; font-weight:bold; background:${visBadgeBg}; color:${visBadgeColor};">${visText}</span></td>
            <td style="padding:15px;">${ev.location}</td>
            <td style="padding:15px;">${new Date(ev.eventDate).toLocaleString()}</td>
            <td style="padding:15px; display: flex; gap: 10px;">
                <button onclick="viewParticipants(${ev.id})" class="action-btn view" style="background:#eef2ff; color:#4318ff; border:none; padding:8px; border-radius:8px; cursor:pointer;" title="View Participants">
                    <i class="bi bi-eye"></i>
                </button>
                <button onclick="editEvent(${ev.id})" class="action-btn edit" style="background:#fff7ed; color:#f59e0b; border:none; padding:8px; border-radius:8px; cursor:pointer;" title="Edit Event">
                    <i class="bi bi-pencil-square"></i>
                </button>
                <button onclick="deleteEvent(${ev.id})" class="action-btn delete" style="background:#fef2f2; color:#ef4444; border:none; padding:8px; border-radius:8px; cursor:pointer;" title="Delete Event">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>`;
        });
        html += `</tbody></table>`;
        document.getElementById('events-table-div').innerHTML = data.length > 0 ? html : "<p>No events found. Click 'Add New Event' to start.</p>";
    } catch (e) {
        document.getElementById('events-table-div').innerHTML = "Error loading events.";
    }
}

// 1. Edit Function
async function editEvent(id) {
    try {
        const res = await fetch(`${API_BASE}/Events/${id}`);
        if (!res.ok) throw new Error("Event not found");
        const ev = await res.json();

        document.getElementById('eventTitle').value = ev.title;
        document.getElementById('eventType').value = ev.type || 'Other';
        document.getElementById('eventLocation').value = ev.location;
        document.getElementById('eventDesc').value = ev.description || '';
        // Visibility data එක form එකට දැම්මා
        document.getElementById('eventVisibility').value = ev.visibility || 'Public';

        if (ev.eventDate) {
            const dateObj = new Date(ev.eventDate);
            document.getElementById('eventDate').value = dateObj.toISOString().split('T')[0];
            document.getElementById('eventTime').value = dateObj.toTimeString().slice(0, 5);
        }

        let idInput = document.getElementById('currentEventId');
        if (!idInput) {
            idInput = document.createElement('input');
            idInput.type = "hidden";
            idInput.id = "currentEventId";
            document.getElementById('addEventForm').appendChild(idInput);
        }
        idInput.value = id;

        document.querySelector('#eventModal h2').innerText = "Update Event";
        document.querySelector('#eventModal .btn-login').innerHTML = '<i class="bi bi-save"></i> Save Changes';

        document.getElementById('eventModal').style.display = "block";
    } catch (e) {
        console.error(e);
        alert("Error loading event details. API Error.");
    }
}

// 2. View Function
async function viewParticipants(eventId) {
    try {
        const res = await fetch(`${API_BASE}/Participants`);

        const listDiv = document.getElementById('participants-list');
        listDiv.innerHTML = "<p>Loading participants...</p>";

        document.getElementById('viewModal').style.display = "block";

        if (!res.ok) throw new Error("API Connection Failed");
        const allParts = await res.json();

        const eventParts = allParts.filter(p => p.eventId === eventId);

        if (eventParts.length === 0) {
            listDiv.innerHTML = `
                <div style="text-align:center; padding: 20px; color: var(--text-sec);">
                    <i class="bi bi-person-x" style="font-size: 3rem;"></i>
                    <p>No participants have registered for this event yet.</p>
                </div>`;
        } else {
            let html = `<ul style="list-style:none; padding:0; margin:0;">`;
            eventParts.forEach(p => {
                html += `
                    <li style="padding: 15px; border-bottom: 1px solid #eee; display: flex; align-items: center; gap: 15px;">
                        <div style="width: 40px; height: 40px; background: var(--bg); border-radius: 50%; display: flex; justify-content: center; align-items: center; color: var(--accent); font-weight: bold;">
                            ${p.fullName.charAt(0).toUpperCase()}
                        </div>
                        <div>
                            <strong style="display: block; color: var(--text-main);">${p.fullName}</strong>
                            <span style="color: var(--text-sec); font-size: 0.9rem;">${p.email}</span>
                        </div>
                    </li>`;
            });
            html += `</ul>`;
            listDiv.innerHTML = html;
        }

    } catch (e) {
        document.getElementById('participants-list').innerHTML = "<p style='color:red;'>Error loading data. Is the Participants API running?</p>";
    }
}

// 3. Delete Function
async function deleteEvent(id) {
    if (confirm("Are you sure you want to delete this event?")) {
        try {
            const res = await fetch(`${API_BASE}/Events/${id}`, { method: 'DELETE' });
            if (res.ok) {
                alert("Event deleted successfully!");
                showSection('events');
            }
        } catch (err) { console.error(err); }
    }
}

async function loadDashboardStats() {
    const email = localStorage.getItem('userEmail');
    try {
        const res = await fetch(`${API_BASE}/Events/ByOrganizer/${email}`);
        const events = await res.json();
        document.getElementById('main-display-area').innerHTML = `
            <div style="display:flex; gap:20px;">
                <div class="card" style="flex:1;"><h3>${events.length}</h3><p>Your Events</p></div>
                <div class="card" style="flex:1;"><h3 style="color:#10b981;">Active</h3><p>Service Status</p></div>
            </div>`;
    } catch (e) { console.error(e); }
}

function logout() {
    localStorage.clear();
    window.location.href = "login.html";
}

document.addEventListener('DOMContentLoaded', () => {
    if (window.location.pathname.includes('dashboard.html')) {
        if (localStorage.getItem('isLoggedIn') !== 'true') {
            window.location.href = "login.html";
            return;
        }
        document.getElementById('user-display').innerText = "Logged in as: " + localStorage.getItem('userEmail');
        showSection('dashboard');
    }
});