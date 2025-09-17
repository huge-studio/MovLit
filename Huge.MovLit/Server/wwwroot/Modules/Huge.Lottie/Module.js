function playWhenReady(elem) {
    if (!elem) return;

    try { elem.play?.(); } catch { }

    const onReady = () => {
        try { elem.play?.(); } finally {
            elem.removeEventListener('ready', onReady);
        }
    };
    elem.addEventListener?.('ready', onReady, { once: true });
}

window.playWhenReady = playWhenReady;