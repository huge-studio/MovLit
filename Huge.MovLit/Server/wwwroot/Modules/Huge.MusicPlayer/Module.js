export function play(audioElement) {
    if (audioElement) {
        audioElement.play().catch(error => {
            console.log('Audio play failed:', error);
        });
    }
}

export function pause(audioElement) {
    if (audioElement) {
        audioElement.pause();
    }
}

export function setMuted(audioElement, muted) {
    if (audioElement) {
        audioElement.muted = muted;
    }
}

export function setLoop(audioElement, loop) {
    if (audioElement) {
        audioElement.loop = loop;
    }
}
