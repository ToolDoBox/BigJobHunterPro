// ================================================
// BIG JOB HUNTER PRO - JAVASCRIPT
// Arcade Interactions & Animations
// ================================================

// ================================================
// SMOOTH SCROLL
// ================================================
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// ================================================
// MODAL FUNCTIONALITY
// ================================================
function showComingSoon() {
    const modal = document.getElementById('comingSoonModal');
    modal.classList.add('active');

    // Play a quick arcade sound effect (optional - commented out)
    // playArcadeSound();
}

function closeModal() {
    const modal = document.getElementById('comingSoonModal');
    modal.classList.remove('active');
}

// Close modal when clicking outside
window.addEventListener('click', function(event) {
    const modal = document.getElementById('comingSoonModal');
    if (event.target === modal) {
        closeModal();
    }
});

// Close modal with ESC key
document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        closeModal();
    }
});

// ================================================
// ANIMATED COUNTERS (Stats Section)
// ================================================
function animateCounter(element, target, duration = 2000) {
    const start = 0;
    const increment = target / (duration / 16); // 60fps
    let current = start;

    const timer = setInterval(() => {
        current += increment;
        if (current >= target) {
            element.textContent = target;
            clearInterval(timer);
        } else {
            element.textContent = Math.floor(current);
        }
    }, 16);
}

// Intersection Observer for counter animation
const observerOptions = {
    threshold: 0.5,
    rootMargin: '0px'
};

const counterObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting && !entry.target.classList.contains('animated')) {
            const target = parseInt(entry.target.getAttribute('data-target'));
            animateCounter(entry.target, target);
            entry.target.classList.add('animated');
        }
    });
}, observerOptions);

// Observe all stat numbers
document.addEventListener('DOMContentLoaded', () => {
    const statNumbers = document.querySelectorAll('.stat-number');
    statNumbers.forEach(stat => counterObserver.observe(stat));
});

// ================================================
// PARALLAX SCROLL EFFECT
// ================================================
let lastScrollY = window.scrollY;

window.addEventListener('scroll', () => {
    const scrollY = window.scrollY;

    // Parallax on hero image
    const heroImage = document.querySelector('.hero-image');
    if (heroImage) {
        const offset = scrollY * 0.3;
        heroImage.style.transform = `translateY(${offset}px)`;
    }

    lastScrollY = scrollY;
});

// ================================================
// FEATURE CARDS STAGGER ANIMATION
// ================================================
const cardObserver = new IntersectionObserver((entries) => {
    entries.forEach((entry, index) => {
        if (entry.isIntersecting) {
            setTimeout(() => {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }, index * 100);
            cardObserver.unobserve(entry.target);
        }
    });
}, {
    threshold: 0.2
});

document.addEventListener('DOMContentLoaded', () => {
    const cards = document.querySelectorAll('.feature-card');
    cards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        cardObserver.observe(card);
    });
});

// ================================================
// SCREENSHOT HOVER EFFECT (adds extra arcade feel)
// ================================================
document.addEventListener('DOMContentLoaded', () => {
    const screenshots = document.querySelectorAll('.screenshot-item');

    screenshots.forEach(item => {
        const img = item.querySelector('.screenshot');

        item.addEventListener('mouseenter', () => {
            img.style.filter = 'brightness(1.1) contrast(1.05)';
        });

        item.addEventListener('mouseleave', () => {
            img.style.filter = 'brightness(1) contrast(1)';
        });
    });
});

// ================================================
// EASTER EGG: KONAMI CODE
// ================================================
let konamiCode = [];
const konamiPattern = ['ArrowUp', 'ArrowUp', 'ArrowDown', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'ArrowLeft', 'ArrowRight', 'b', 'a'];

document.addEventListener('keydown', (e) => {
    konamiCode.push(e.key);
    konamiCode = konamiCode.slice(-10);

    if (konamiCode.join(',') === konamiPattern.join(',')) {
        activateEasterEgg();
        konamiCode = [];
    }
});

function activateEasterEgg() {
    // Activate super arcade mode
    document.body.style.animation = 'rainbow-bg 2s infinite';

    // Add rainbow animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes rainbow-bg {
            0% { filter: hue-rotate(0deg); }
            100% { filter: hue-rotate(360deg); }
        }
    `;
    document.head.appendChild(style);

    // Show special message
    const msg = document.createElement('div');
    msg.style.cssText = `
        position: fixed;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        font-family: 'Press Start 2P', monospace;
        font-size: 1.5rem;
        color: #FFB000;
        text-shadow: 0 0 20px #FFB000;
        z-index: 10001;
        text-align: center;
        padding: 2rem;
        background: rgba(0,0,0,0.9);
        border: 4px solid #FF6700;
    `;
    msg.textContent = '🎮 ARCADE MODE ACTIVATED! 🎮';
    document.body.appendChild(msg);

    setTimeout(() => {
        msg.remove();
        document.body.style.animation = '';
    }, 3000);
}

// ================================================
// RANDOM ARCADE GLITCH EFFECT (subtle)
// ================================================
function randomGlitch() {
    const title = document.querySelector('.pixel-title');
    if (title && Math.random() < 0.1) { // 10% chance
        title.style.transform = `translate(${Math.random() * 4 - 2}px, ${Math.random() * 4 - 2}px)`;
        setTimeout(() => {
            title.style.transform = 'translate(0, 0)';
        }, 50);
    }
}

// Run glitch effect occasionally
setInterval(randomGlitch, 5000);

// ================================================
// BUTTON ARCADE SOUND (Optional - requires audio files)
// ================================================
function playArcadeSound() {
    // Uncomment and add your own arcade sound file
    // const audio = new Audio('sounds/arcade-blip.mp3');
    // audio.volume = 0.3;
    // audio.play();
}

// Add subtle sound to all arcade buttons
document.addEventListener('DOMContentLoaded', () => {
    const buttons = document.querySelectorAll('.arcade-button');
    buttons.forEach(button => {
        button.addEventListener('click', () => {
            // Visual feedback
            button.style.transform = 'scale(0.95)';
            setTimeout(() => {
                button.style.transform = '';
            }, 100);
        });
    });
});

// ================================================
// PERFORMANCE: Reduce animations on low-end devices
// ================================================
if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    document.querySelectorAll('*').forEach(el => {
        el.style.animation = 'none';
        el.style.transition = 'none';
    });
}

// ================================================
// LOG LOADED MESSAGE (Arcade style)
// ================================================
console.log('%c🎮 BIG JOB HUNTER PRO 🎮',
    'font-size: 20px; font-weight: bold; color: #FF6700; text-shadow: 0 0 10px #FF6700;');
console.log('%c>> System initialized. Ready to hunt.',
    'font-size: 12px; color: #00FF00; font-family: monospace;');
console.log('%cTry the Konami Code: ↑↑↓↓←→←→BA',
    'font-size: 10px; color: #FFB000; font-family: monospace;');
