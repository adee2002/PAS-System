// PAS - Project Approval System
// Custom JavaScript for enhanced user experience

// Wait for DOM to load
document.addEventListener('DOMContentLoaded', function () {

    // Initialize tooltips
    initializeTooltips();

    // Initialize fade-in animations
    initializeAnimations();

    // Add loading state to forms
    initializeFormLoading();

    // Add confirmation dialogs
    initializeConfirmDialogs();

    // Add smooth scroll
    initializeSmoothScroll();
});

// ========== TOOLTIP INITIALIZATION ==========
function initializeTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// ========== ANIMATION INITIALIZATION ==========
function initializeAnimations() {
    // Add fade-in class to main content
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.classList.add('fade-in-up');
    }

    // Add slide-in animations to cards
    const cards = document.querySelectorAll('.card-custom, .stat-card, .project-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        setTimeout(() => {
            card.style.transition = 'all 0.6s ease-out';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, index * 100);
    });
}

// ========== FORM LOADING STATE ==========
function initializeFormLoading() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                const originalText = submitBtn.innerHTML;
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';

                // Re-enable after 30 seconds (in case of error)
                setTimeout(() => {
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = originalText;
                }, 30000);
            }
        });
    });
}

// ========== CONFIRM DIALOGS ==========
function initializeConfirmDialogs() {
    const confirmButtons = document.querySelectorAll('[data-confirm]');
    confirmButtons.forEach(btn => {
        btn.addEventListener('click', function (e) {
            const message = this.getAttribute('data-confirm') || 'Are you sure?';
            if (!confirm(message)) {
                e.preventDefault();
                return false;
            }
        });
    });
}

// ========== SMOOTH SCROLL ==========
function initializeSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// ========== SHOW TOAST NOTIFICATION ==========
function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        // Create toast container if it doesn't exist
        const container = document.createElement('div');
        container.id = 'toast-container';
        container.style.position = 'fixed';
        container.style.bottom = '20px';
        container.style.right = '20px';
        container.style.zIndex = '9999';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;

    document.getElementById('toast-container').appendChild(toast);
    const bsToast = new bootstrap.Toast(toast, { delay: 3000 });
    bsToast.show();

    // Remove toast after it's hidden
    toast.addEventListener('hidden.bs.toast', () => {
        toast.remove();
    });
}

// ========== SEARCH/FILTER FUNCTIONALITY ==========
function filterTable(inputId, tableId) {
    const input = document.getElementById(inputId);
    if (!input) return;

    input.addEventListener('keyup', function () {
        const filter = this.value.toLowerCase();
        const table = document.getElementById(tableId);
        const rows = table.getElementsByTagName('tr');

        for (let i = 1; i < rows.length; i++) {
            let found = false;
            const cells = rows[i].getElementsByTagName('td');
            for (let j = 0; j < cells.length; j++) {
                const cellText = cells[j].textContent || cells[j].innerText;
                if (cellText.toLowerCase().indexOf(filter) > -1) {
                    found = true;
                    break;
                }
            }
            rows[i].style.display = found ? '' : 'none';
        }
    });
}

// ========== PROJECT STATUS COLOR HELPER ==========
function getStatusBadge(status) {
    const statusMap = {
        'Pending': '<span class="badge badge-custom-pending"><i class="fas fa-clock"></i> Pending</span>',
        'UnderReview': '<span class="badge badge-custom-review"><i class="fas fa-search"></i> Under Review</span>',
        'Under Review': '<span class="badge badge-custom-review"><i class="fas fa-search"></i> Under Review</span>',
        'Matched': '<span class="badge badge-custom-matched"><i class="fas fa-check-circle"></i> Matched</span>',
        'Withdrawn': '<span class="badge bg-secondary"><i class="fas fa-times-circle"></i> Withdrawn</span>',
        'Confirmed': '<span class="badge badge-custom-approved"><i class="fas fa-check"></i> Confirmed</span>'
    };

    return statusMap[status] || '<span class="badge bg-secondary">' + status + '</span>';
}

// ========== COUNTDOWN TIMER ==========
function startCountdown(elementId, seconds) {
    let remaining = seconds;
    const element = document.getElementById(elementId);
    if (!element) return;

    const interval = setInterval(() => {
        if (remaining <= 0) {
            clearInterval(interval);
            element.innerHTML = 'Time is up!';
            element.style.color = '#dc3545';
        } else {
            const minutes = Math.floor(remaining / 60);
            const secs = remaining % 60;
            element.innerHTML = `${minutes}:${secs.toString().padStart(2, '0')}`;
            remaining--;
        }
    }, 1000);
}

// ========== CHARACTER COUNTER FOR TEXTAREAS ==========
function addCharacterCounter(textareaId, maxLength) {
    const textarea = document.getElementById(textareaId);
    if (!textarea) return;

    const counter = document.createElement('div');
    counter.className = 'text-muted mt-1 small';
    counter.style.textAlign = 'right';
    textarea.parentNode.insertBefore(counter, textarea.nextSibling);

    function updateCounter() {
        const remaining = maxLength - textarea.value.length;
        counter.innerHTML = `${remaining} characters remaining`;
        if (remaining < 0) {
            counter.style.color = '#dc3545';
        } else if (remaining < 50) {
            counter.style.color = '#ffc107';
        } else {
            counter.style.color = '#6c757d';
        }
    }

    textarea.addEventListener('input', updateCounter);
    updateCounter();
}

// ========== AUTO-HIDE ALERTS ==========
function autoHideAlerts() {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s ease';
            alert.style.opacity = '0';
            setTimeout(() => {
                alert.remove();
            }, 500);
        }, 5000);
    });
}

// ========== DARK MODE TOGGLE (OPTIONAL) ==========
function toggleDarkMode() {
    const body = document.body;
    body.classList.toggle('dark-mode');
    const isDark = body.classList.contains('dark-mode');
    localStorage.setItem('darkMode', isDark);
}

function loadDarkModePreference() {
    const isDark = localStorage.getItem('darkMode') === 'true';
    if (isDark) {
        document.body.classList.add('dark-mode');
    }
}

// Call auto-hide alerts on page load
autoHideAlerts();