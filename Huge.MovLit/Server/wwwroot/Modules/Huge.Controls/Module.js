// Fullscreen toggle functions
export function setFullscreen(isFullscreen) {
    const navbar = document.querySelector('.navbar');
    const content = document.querySelector('.content');
    
    if (isFullscreen) {
        // Hide navbar
        if (navbar) {
            navbar.style.display = 'none';
        }
        
        // Adjust content padding
        if (content) {
            // Check if we're on small screen (< 992px) or large screen
            const isSmallScreen = window.innerWidth < 992;
            if (isSmallScreen) {
                content.style.paddingTop = '10rem';
            } else {
                content.style.paddingTop = '6rem';
            }
        }
    } else {
        // Show navbar
        if (navbar) {
            navbar.style.display = '';
        }
        
        // Reset content padding to original values
        if (content) {
            const isSmallScreen = window.innerWidth < 992;
            if (isSmallScreen) {
                content.style.paddingTop = '13rem';
            } else {
                content.style.paddingTop = '9rem';
            }
        }
    }
}

// Audio player functions
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
