import { Link } from 'react-router-dom';

const featureCards = [
    {
        title: '10-Second Quick Capture',
        body: 'Paste the job URL and page text, earn points instantly, let AI fill the details.',
    },
    {
        title: 'Hunting Party Rivalry',
        body: 'Compete with friends, see who is ahead, and close the gap with daily wins.',
    },
    {
        title: 'Import Your History',
        body: 'Bring in your Job-Hunt-Context JSON files with retroactive points.',
    },
];

const steps = [
    {
        label: 'Step 01',
        title: 'Log the hunt',
        body: 'Paste the job posting, lock it in, move on in seconds.',
    },
    {
        label: 'Step 02',
        title: 'Score the effort',
        body: 'Applications, interviews, and rejections all count toward momentum.',
    },
    {
        label: 'Step 03',
        title: 'Hunt together',
        body: 'Create a party, climb the leaderboard, stay accountable.',
    },
];

const highlights = [
    {
        label: 'Applied',
        value: '+1',
    },
    {
        label: 'Interview',
        value: '+5',
    },
    {
        label: 'Offer',
        value: '+50',
    },
];

export default function Landing() {
    return (
        <div className="min-h-screen bg-metal-dark app-shell">
            <header className="header-bar">
                <div className="max-w-6xl mx-auto px-6 py-4 flex flex-wrap items-center justify-between gap-4">
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
                <section className="max-w-6xl mx-auto px-6 py-16 grid lg:grid-cols-2 gap-12 items-center">
                    <div className="space-y-6">
                        <p className="pixel-text-sm text-terminal">Friend Group Alpha</p>
                        <h1 className="text-3xl sm:text-4xl leading-tight">
                            The job hunt is brutal.
                            <br />
                            Make it competitive.
                        </h1>
                        <p className="text-gray-300">
                            Big Job Hunter Pro turns job applications into a collaborative arcade.
                            Log applications fast, score the effort, and keep your crew moving
                            when burnout hits.
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
                        <div className="flex flex-wrap gap-4">
                            {highlights.map((item) => (
                                <div key={item.label} className="stat-display w-32 text-center">
                                    <div className="stat-display-value">{item.value}</div>
                                    <div className="stat-display-label">{item.label}</div>
                                </div>
                            ))}
                        </div>
                    </div>
                    <div className="hud-frame p-6">
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
                    </div>
                </section>

                <section className="max-w-6xl mx-auto px-6 pb-12">
                    <div className="grid md:grid-cols-3 gap-6">
                        {featureCards.map((feature) => (
                            <div key={feature.title} className="metal-panel">
                                <div className="metal-panel-screws" />
                                <h3 className="font-arcade text-base text-amber mb-3">
                                    {feature.title}
                                </h3>
                                <p className="text-gray-300 text-sm">{feature.body}</p>
                            </div>
                        ))}
                    </div>
                </section>

                <section className="max-w-6xl mx-auto px-6 py-12">
                    <div className="grid lg:grid-cols-[1.2fr_1fr] gap-8 items-center">
                        <div className="space-y-4">
                            <h2 className="text-2xl">Built for friend groups in the trenches</h2>
                            <p className="text-gray-300">
                                We built this because our own crew needed daily accountability.
                                It is designed for fast logging, shared progress, and the
                                friendly pressure that keeps everyone moving.
                            </p>
                            <p className="text-gray-400 text-sm">
                                Phase 1 ships with hunting parties, leaderboards, and a JSON
                                import tool for Job-Hunt-Context.
                            </p>
                        </div>
                        <div className="metal-panel bg-metal">
                            <div className="metal-panel-screws" />
                            <h3 className="font-arcade text-base text-amber mb-4">Why it works</h3>
                            <ul className="space-y-3 text-sm text-gray-300">
                                <li>Accountability beats isolation.</li>
                                <li>Points reward effort, not just outcomes.</li>
                                <li>Quick capture keeps momentum alive.</li>
                            </ul>
                        </div>
                    </div>
                </section>

                <section className="max-w-6xl mx-auto px-6 py-12">
                    <div className="metal-panel">
                        <div className="metal-panel-screws" />
                        <div className="flex flex-wrap items-center justify-between gap-6">
                            <div className="space-y-3">
                                <h2 className="text-2xl">How the hunt flows</h2>
                                <p className="text-gray-300 text-sm">
                                    A simple loop you can run every day, even on low energy.
                                </p>
                            </div>
                            <Link className="btn-metal-primary" to="/register">
                                Start in 60 Seconds
                            </Link>
                        </div>
                        <div className="grid md:grid-cols-3 gap-6 mt-8">
                            {steps.map((step) => (
                                <div key={step.label} className="bg-metal-dark p-4 rounded-lg border border-metal">
                                    <p className="pixel-text-sm text-terminal">{step.label}</p>
                                    <h3 className="font-arcade text-sm text-amber mt-2">
                                        {step.title}
                                    </h3>
                                    <p className="text-gray-300 text-sm mt-2">
                                        {step.body}
                                    </p>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                <section className="max-w-6xl mx-auto px-6 py-16">
                    <div className="hud-frame p-8 text-center">
                        <p className="pixel-text-sm text-terminal">Beta Access</p>
                        <h2 className="text-2xl mt-4">
                            Ready to turn burnout into momentum?
                        </h2>
                        <p className="text-gray-300 mt-4 max-w-2xl mx-auto">
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

            <footer className="hud-footer py-6">
                <div className="max-w-6xl mx-auto px-6 flex flex-wrap items-center justify-between gap-4 text-xs text-gray-500">
                    <span>Big Job Hunter Pro · built by a friend group in the trenches</span>
                    <span>© 2026 BigJobHunter.pro</span>
                </div>
            </footer>
        </div>
    );
}
