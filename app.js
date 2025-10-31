// Main App Logic for 67 Tetris
const trackEvent = (eventName, eventData = {}) => {
    if (typeof window !== 'undefined' && typeof window.va === 'function') {
        window.va(eventName, eventData);
    }
};

// Initialize analytics tracking when page loads
window.addEventListener('load', () => {
    trackEvent('page_loaded');
});
