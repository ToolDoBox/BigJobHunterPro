import { Link } from 'react-router-dom';
import { useEffect, useRef, useState } from 'react';

// Hook for scroll-triggered fade-in animations
function useScrollFadeIn() {
    const ref = useRef<HTMLDivElement>(null);
    const [isVisible, setIsVisible] = useState(false);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting) {
                    setIsVisible(true);
                    observer.disconnect();
                }
            },
            { threshold: 0.1, rootMargin: '0px 0px -50px 0px' }
        );

        if (ref.current) {
            observer.observe(ref.current);
        }

        return () => observer.disconnect();
    }, []);

    return { ref, isVisible };
}

// Screenshot data for gallery
const screenshots = [
    {
        id: 'dashboard',
        src: '/screenshots/landing-dashboard.png',
        alt: 'The Lodge - Your hunting command center',
        caption: 'The Lodge',
        description: 'Stats, weekly progress, and analytics at a glance',
    },
    {
        id: 'hunting-party',
        src: '/screenshots/landing-hunting-party.png',
        alt: 'Hunting Party Leaderboard',
        caption: 'Hunting Party',
        description: 'Compete with friends on live leaderboards',
    },
    {
        id: 'quick-capture',
        src: '/screenshots/landing-quick-capture.png',
        alt: 'Quick Capture Modal',
        caption: 'Quick Capture',
        description: 'Log applications in 15 seconds with Ctrl+K',
    },
    {
        id: 'application-detail',
        src: '/screenshots/landing-application-detail.png',
        alt: 'Application Detail View',
        caption: 'Full Tracking',
        description: 'Every detail from company info to timeline',
    },
    {
        id: 'timeline',
        src: '/screenshots/landing-timeline.png',
        alt: 'Timeline Events',
        caption: 'Timeline',
        description: 'Track every call, email, and interview',
    },
    {
        id: 'analytics',
        src: '/screenshots/landing-analytics.png',
        alt: 'Analytics Dashboard',
        caption: 'Analytics',
        description: 'Know what sources and keywords work best',
    },
    {
        id: 'charts',
        src: '/screenshots/landing-charts.png',
        alt: 'Application Breakdown Charts',
        caption: 'Charts',
        description: 'Pie charts showing status and source breakdown',
    },
    {
        id: 'contacts',
        src: '/screenshots/landing-contacts.png',
        alt: 'Contact Management',
        caption: 'Contacts',
        description: 'Store recruiters and hiring managers per app',
    },
    {
        id: 'cover-letter',
        src: '/screenshots/landing-cover-letter.png',
        alt: 'AI Cover Letter Generator',
        caption: 'Cover Letters',
        description: 'Generate tailored cover letters with AI',
    },
];

// Expanded feature cards - organized by category
const featureCategories = [
    {
        title: 'Track Everything',
        features: [
            {
                title: 'Quick Capture',
                body: 'Log applications in 15 seconds. Paste the URL, fill 3 fields, done. Ctrl+K and go.',
                icon: '⚡',
            },
            {
                title: 'Timeline Tracking',
                body: 'Every email, call, and interview logged in chronological order. Never lose track.',
                icon: '📅',
            },
            {
                title: 'Contact Management',
                body: 'Store recruiters, hiring managers, and team contacts per application.',
                icon: '👤',
            },
            {
                title: 'Cover Letters',
                body: 'Generate and store tailored cover letters for each role. Export to PDF.',
                icon: '📝',
            },
        ],
    },
    {
        title: 'Analyze & Improve',
        features: [
            {
                title: 'Analytics Dashboard',
                body: 'See weekly progress, top keywords, and conversion rates at a glance.',
                icon: '📊',
            },
            {
                title: 'Source Tracking',
                body: 'Know which job boards yield the most interviews. Double down on what works.',
                icon: '🎯',
            },
            {
                title: 'Streak System',
                body: 'Build daily momentum with streak tracking. Resilience points for rejections.',
                icon: '🔥',
            },
            {
                title: 'Hunting Parties',
                body: 'Compete with friends on shared leaderboards. Stay accountable together.',
                icon: '🏆',
            },
        ],
    },
];

// Points system breakdown
const pointsSystem = [
    { action: 'Applied', points: '+1', description: 'Every application counts' },
    { action: 'Screening', points: '+2', description: 'Phone screens move you forward' },
    { action: 'Interview', points: '+5', description: 'Face time with the team' },
    { action: 'Rejection', points: '+2', description: 'Resilience points - keep going' },
    { action: 'Offer', points: '+50', description: 'The ultimate win' },
];

// How it works steps
const steps = [
    {
        number: '01',
        title: 'Log the Hunt',
        body: 'Paste the job posting, hit Ctrl+K, fill 3 fields. You\'re done in 15 seconds.',
    },
    {
        number: '02',
        title: 'Track the Journey',
        body: 'Timeline events, contacts, notes, and cover letters - all in one place.',
    },
    {
        number: '03',
        title: 'Analyze What Works',
        body: 'See which sources convert, which keywords land interviews, and where to focus.',
    },
    {
        number: '04',
        title: 'Stay Accountable',
        body: 'Daily streaks, points for every action, and friends competing alongside you.',
    },
];

// Hero highlights
const heroHighlights = [
    { label: 'Applied', value: '+1' },
    { label: 'Interview', value: '+5' },
    { label: 'Offer', value: '+50' },
];

export default function Landing() {
    const heroFade = useScrollFadeIn();
    const screenshotsFade = useScrollFadeIn();
    const featuresFade = useScrollFadeIn();
    const pointsFade = useScrollFadeIn();
    const stepsFade = useScrollFadeIn();
    const socialFade = useScrollFadeIn();
    const ctaFade = useScrollFadeIn();

    const [activeScreenshot, setActiveScreenshot] = useState(0);
    const [imageErrors, setImageErrors] = useState<Set<string>>(new Set());

    const handleImageError = (id: string) => {
        setImageErrors((prev) => new Set(prev).add(id));
    };

    return (
        <div className="min-h-screen bg-metal-dark app-shell">
            {/* Header */}
            <header className="header-bar">
                <div className="max-w-6xl mx-auto px-4 sm:px-6 py-3 sm:py-4 flex flex-wrap items-center justify-between gap-3 sm:gap-4">
                    <div className="flex items-center gap-3">
                        <span className="logo-badge font-arcade text-xs text-amber">
                            BJH PRO
                        </span>
                        <span className="font-arcade text-xs text-gray-300">
                            Big Job Hunter Pro
                        </span>
                    </div>
                    <nav className="flex items-center gap-3 text-xs">
                        <Link className="nav-link font-arcade text-gray-200" to="/login">
                            Login
                        </Link>
                        <Link className="btn-metal-primary" to="/register">
                            Start the Hunt
                        </Link>
                    </nav>
                </div>
            </header>

            <main>
                {/* Section 1: Hero */}
                <section
                    ref={heroFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 lg:py-16 grid lg:grid-cols-2 gap-8 lg:gap-12 items-center scroll-fade-in ${heroFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="space-y-4 sm:space-y-6">
                        <p className="pixel-text-sm text-terminal">Complete Job Search Command Center</p>
                        <h1 className="text-2xl sm:text-3xl lg:text-4xl leading-tight">
                            The job hunt is brutal.
                            <br />
                            Make it competitive.
                        </h1>
                        <p className="text-gray-300 text-sm sm:text-base">
                            Big Job Hunter Pro is a complete job search command center. Track every
                            application, analyze what's working, and compete with friends - all in
                            one retro-styled dashboard built for speed.
                        </p>
                        <div className="flex flex-wrap gap-4">
                            <Link className="btn-metal-primary" to="/register">
                                Join the Beta
                            </Link>
                            <Link className="btn-metal" to="/login">
                                I Have an Account
                            </Link>
                        </div>
                        <div className="divider-metal" />
                        <div className="flex flex-wrap gap-3 sm:gap-4">
                            {heroHighlights.map((item) => (
                                <div key={item.label} className="stat-display w-24 sm:w-32 text-center">
                                    <div className="stat-display-value text-sm sm:text-base">{item.value}</div>
                                    <div className="stat-display-label">{item.label}</div>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className="hud-frame p-2 sm:p-4">
                        {!imageErrors.has('hero') ? (
                            <img
                                src="/screenshots/landing-dashboard.png"
                                alt="Big Job Hunter Pro Dashboard"
                                className="w-full rounded-lg border border-metal-border"
                                onError={() => handleImageError('hero')}
                            />
                        ) : (
                            <div className="metal-panel metal-panel-orange">
                                <div className="metal-panel-screws" />
                                <h2 className="font-arcade text-lg text-amber mb-4">
                                    HUNT CONSOLE
                                </h2>
                                <div className="space-y-4 text-sm text-gray-200">
                                    <div>
                                        <p className="pixel-text-sm text-terminal">Quick Capture</p>
                                        <p className="text-gray-300">
                                            Paste URL + page content. AI locks in the details
                                            while you keep applying.
                                        </p>
                                    </div>
                                    <div>
                                        <p className="pixel-text-sm text-terminal">Rivalry Panel</p>
                                        <p className="text-gray-300">
                                            See the teammate ahead and the exact gap to close.
                                        </p>
                                    </div>
                                    <div>
                                        <p className="pixel-text-sm text-terminal">Resilience Points</p>
                                        <p className="text-gray-300">
                                            Rejections count. Momentum stays visible.
                                        </p>
                                    </div>
                                </div>
                                <div className="mt-6 flex items-center justify-between text-xs text-gray-400">
                                    <span>Next sync: 00:12</span>
                                    <span className="pixel-text-sm text-amber">READY</span>
                                </div>
                            </div>
                        )}
                    </div>
                </section>

                {/* Section 2: Screenshot Gallery */}
                <section
                    ref={screenshotsFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 scroll-fade-in ${screenshotsFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="text-center mb-6 sm:mb-8">
                        <p className="pixel-text-sm text-terminal mb-2">See It In Action</p>
                        <h2 className="text-xl sm:text-2xl">Built for hunters, by hunters</h2>
                    </div>

                    {/* Screenshot carousel */}
                    <div className="hud-frame p-3 sm:p-4 lg:p-6">
                        <div className="relative">
                            {/* Main screenshot */}
                            <div className="aspect-video bg-metal-dark rounded-lg overflow-hidden border border-metal-border">
                                {!imageErrors.has(screenshots[activeScreenshot].id) ? (
                                    <img
                                        src={screenshots[activeScreenshot].src}
                                        alt={screenshots[activeScreenshot].alt}
                                        className="w-full h-full object-cover object-top"
                                        onError={() => handleImageError(screenshots[activeScreenshot].id)}
                                    />
                                ) : (
                                    <div className="w-full h-full flex items-center justify-center bg-metal">
                                        <div className="text-center">
                                            <p className="font-arcade text-amber text-lg mb-2">
                                                {screenshots[activeScreenshot].caption}
                                            </p>
                                            <p className="text-gray-400 text-sm">
                                                {screenshots[activeScreenshot].description}
                                            </p>
                                            <p className="text-gray-500 text-xs mt-4">
                                                Screenshot coming soon
                                            </p>
                                        </div>
                                    </div>
                                )}
                            </div>

                            {/* Caption */}
                            <div className="mt-3 sm:mt-4 text-center">
                                <p className="font-arcade text-amber text-xs sm:text-sm">
                                    {screenshots[activeScreenshot].caption}
                                </p>
                                <p className="text-gray-400 text-xs sm:text-sm mt-1">
                                    {screenshots[activeScreenshot].description}
                                </p>
                            </div>
                        </div>

                        {/* Thumbnail navigation */}
                        <div className="flex flex-wrap justify-center gap-2 sm:gap-3 mt-6 max-w-4xl mx-auto">
                            {screenshots.map((screenshot, index) => (
                                <button
                                    key={screenshot.id}
                                    onClick={() => setActiveScreenshot(index)}
                                    className={`px-2 py-1.5 sm:px-4 sm:py-2 rounded-lg border-2 transition-all ${
                                        index === activeScreenshot
                                            ? 'border-blaze bg-metal-light text-amber'
                                            : 'border-metal-border bg-metal-dark text-gray-400 hover:border-metal-highlight'
                                    }`}
                                >
                                    <span className="font-arcade text-[8px] sm:text-xs">{screenshot.caption}</span>
                                </button>
                            ))}
                        </div>
                    </div>
                </section>

                {/* Section 3: Feature Showcase */}
                <section
                    ref={featuresFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 scroll-fade-in ${featuresFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="text-center mb-6 sm:mb-10">
                        <p className="pixel-text-sm text-terminal mb-2">Everything You Need</p>
                        <h2 className="text-xl sm:text-2xl">Features built for the trenches</h2>
                    </div>

                    {featureCategories.map((category) => (
                        <div key={category.title} className="mb-10">
                            <h3 className="font-arcade text-sm text-terminal mb-4 uppercase">
                                {category.title}
                            </h3>
                            <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-4">
                                {category.features.map((feature) => (
                                    <div key={feature.title} className="metal-panel">
                                        <div className="metal-panel-screws" />
                                        <div className="text-2xl mb-3">{feature.icon}</div>
                                        <h4 className="font-arcade text-sm text-amber mb-2">
                                            {feature.title}
                                        </h4>
                                        <p className="text-gray-300 text-sm">{feature.body}</p>
                                    </div>
                                ))}
                            </div>
                        </div>
                    ))}
                </section>

                {/* Section 4: Points System */}
                <section
                    ref={pointsFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 scroll-fade-in ${pointsFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="hud-frame p-4 sm:p-6 lg:p-8">
                        <div className="text-center mb-6 sm:mb-8">
                            <p className="pixel-text-sm text-terminal mb-2">Gamified Progress</p>
                            <h2 className="text-xl sm:text-2xl">Every action earns points</h2>
                            <p className="text-gray-400 text-sm sm:text-base mt-2">
                                Even rejections reward resilience. Momentum matters.
                            </p>
                        </div>

                        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4">
                            {pointsSystem.map((item) => (
                                <div
                                    key={item.action}
                                    className="bg-metal-dark p-4 rounded-lg border border-metal-border text-center"
                                >
                                    <div className="stat-display-value text-xl mb-1">
                                        {item.points}
                                    </div>
                                    <div className="font-arcade text-xs text-amber mb-2">
                                        {item.action}
                                    </div>
                                    <p className="text-gray-500 text-xs">{item.description}</p>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                {/* Section 5: How It Works */}
                <section
                    ref={stepsFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 scroll-fade-in ${stepsFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="metal-panel">
                        <div className="metal-panel-screws" />
                        <div className="flex flex-wrap items-center justify-between gap-4 sm:gap-6 mb-6 sm:mb-8">
                            <div className="space-y-2 sm:space-y-3">
                                <h2 className="text-xl sm:text-2xl">How the hunt flows</h2>
                                <p className="text-gray-300 text-sm sm:text-base">
                                    A simple loop you can run every day, even on low energy.
                                </p>
                            </div>
                            <Link className="btn-metal-primary" to="/register">
                                Start in 60 Seconds
                            </Link>
                        </div>
                        <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
                            {steps.map((step) => (
                                <div
                                    key={step.number}
                                    className="bg-metal-dark p-4 rounded-lg border border-metal"
                                >
                                    <p className="pixel-text-sm text-terminal">Step {step.number}</p>
                                    <h3 className="font-arcade text-sm text-amber mt-2">
                                        {step.title}
                                    </h3>
                                    <p className="text-gray-300 text-sm mt-2">{step.body}</p>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                {/* Section 6: Social Proof */}
                <section
                    ref={socialFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 scroll-fade-in ${socialFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="grid lg:grid-cols-[1.2fr_1fr] gap-6 sm:gap-8 items-center">
                        <div className="space-y-3 sm:space-y-4">
                            <h2 className="text-xl sm:text-2xl">Built for friend groups in the trenches</h2>
                            <p className="text-gray-300 text-sm sm:text-base">
                                We built this because our own crew needed daily accountability.
                                It is designed for fast logging, shared progress, and the
                                friendly pressure that keeps everyone moving.
                            </p>
                            <p className="text-gray-400 text-xs sm:text-sm">
                                Phase 1 ships with full tracking, analytics, contacts, cover letters,
                                hunting parties, and leaderboards.
                            </p>
                        </div>
                        <div className="metal-panel bg-metal">
                            <div className="metal-panel-screws" />
                            <h3 className="font-arcade text-base text-amber mb-4">Why it works</h3>
                            <ul className="space-y-3 text-sm text-gray-300">
                                <li>Accountability beats isolation.</li>
                                <li>Points reward effort, not just outcomes.</li>
                                <li>Quick capture keeps momentum alive.</li>
                                <li>Analytics show what's actually working.</li>
                                <li>Everything in one place, not 5 spreadsheets.</li>
                            </ul>
                        </div>
                    </div>
                </section>

                {/* Section 7: Final CTA */}
                <section
                    ref={ctaFade.ref}
                    className={`max-w-6xl mx-auto px-4 sm:px-6 py-8 sm:py-12 lg:py-16 scroll-fade-in ${ctaFade.isVisible ? 'visible' : ''}`}
                >
                    <div className="hud-frame p-6 sm:p-8 text-center">
                        <p className="pixel-text-sm text-terminal">Beta Access</p>
                        <h2 className="text-xl sm:text-2xl mt-3 sm:mt-4">
                            Ready to turn burnout into momentum?
                        </h2>
                        <p className="text-gray-300 text-sm sm:text-base mt-3 sm:mt-4 max-w-2xl mx-auto">
                            Big Job Hunter Pro is free for friend groups while we build it
                            in the open. Grab a spot, invite your crew, and start hunting.
                        </p>
                        <div className="mt-6 flex flex-wrap justify-center gap-4">
                            <Link className="btn-metal-primary" to="/register">
                                Claim Beta Spot
                            </Link>
                            <Link className="btn-metal" to="/login">
                                See the Dashboard
                            </Link>
                        </div>
                    </div>
                </section>
            </main>

            {/* Footer */}
            <footer className="hud-footer py-4 sm:py-6">
                <div className="max-w-6xl mx-auto px-4 sm:px-6 flex flex-wrap items-center justify-between gap-3 sm:gap-4 text-xs text-gray-500">
                    <span>Big Job Hunter Pro - built by a friend group in the trenches</span>
                    <span>2026 BigJobHunter.pro</span>
                </div>
            </footer>
        </div>
    );
}
