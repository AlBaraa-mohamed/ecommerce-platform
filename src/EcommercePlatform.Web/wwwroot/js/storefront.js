// Modern Storefront Interactive Features

// Smooth scroll for anchor links
document.addEventListener('DOMContentLoaded', function() {
    // Add animation on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -100px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe product cards
    const productCards = document.querySelectorAll('.product-card-wrapper');
    productCards.forEach((card, index) => {
        card.style.setProperty('--i', index);
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        observer.observe(card);
    });

    // Animate category cards
    const categoryCards = document.querySelectorAll('.category-card-wrapper');
    categoryCards.forEach((card) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(card);
    });

    // Add to cart animation
    const addToCartForms = document.querySelectorAll('.add-to-cart-form');
    addToCartForms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const button = form.querySelector('.btn-add-cart');
            if (button) {
                button.innerHTML = '<i class="bi bi-check"></i>';
                button.style.background = '#28a745';

                setTimeout(() => {
                    button.innerHTML = '<i class="bi bi-cart-plus"></i>';
                    button.style.background = '';
                }, 1500);
            }
        });
    });

    // Parallax effect on hero section
    const heroSection = document.querySelector('.hero-section');
    if (heroSection) {
        window.addEventListener('scroll', function() {
            const scrolled = window.pageYOffset;
            const parallaxSpeed = 0.5;
            heroSection.style.transform = `translateY(${scrolled * parallaxSpeed}px)`;
        });
    }

    // Product image zoom on hover
    const productImages = document.querySelectorAll('.product-image');
    productImages.forEach(img => {
        img.addEventListener('mouseenter', function() {
            this.style.transform = 'scale(1.1)';
        });
        img.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
        });
    });

    // Category card tilt effect
    const categoryCardsLinks = document.querySelectorAll('.category-card');
    categoryCardsLinks.forEach(card => {
        card.addEventListener('mousemove', function(e) {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            const centerX = rect.width / 2;
            const centerY = rect.height / 2;

            const rotateX = (y - centerY) / 10;
            const rotateY = (centerX - x) / 10;

            card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-10px)`;
        });

        card.addEventListener('mouseleave', function() {
            card.style.transform = '';
        });
    });

    // Loading skeleton effect (for future AJAX)
    function showSkeleton(element) {
        element.classList.add('skeleton');
        setTimeout(() => {
            element.classList.remove('skeleton');
        }, 1000);
    }

    // Price animation on load
    const prices = document.querySelectorAll('.product-price, .product-detail-price');
    prices.forEach(price => {
        const finalPrice = price.textContent;
        const numericPrice = parseFloat(finalPrice.replace('$', ''));

        if (!isNaN(numericPrice)) {
            let currentPrice = 0;
            const increment = numericPrice / 30;

            const animation = setInterval(() => {
                currentPrice += increment;
                if (currentPrice >= numericPrice) {
                    price.textContent = finalPrice;
                    clearInterval(animation);
                } else {
                    price.textContent = '$' + currentPrice.toFixed(2);
                }
            }, 16);
        }
    });
});

// Quick view functionality (placeholder for future implementation)
function quickView(productId) {
    console.log('Quick view for product:', productId);
    // Future: Open modal with product details
}

// Wishlist functionality (placeholder)
function addToWishlist(productId) {
    console.log('Added to wishlist:', productId);
    // Future: Add to wishlist via AJAX
}

// Filter products by category
function filterProducts(category) {
    const products = document.querySelectorAll('.product-card-wrapper');
    products.forEach(product => {
        const productCategory = product.querySelector('.product-category')?.textContent.trim();
        if (category === 'all' || productCategory === category) {
            product.style.display = '';
        } else {
            product.style.display = 'none';
        }
    });
}

// Toast notification helper
function showToast(message, type = 'success') {
    const toast = document.createElement('div');
    toast.className = `toast-notification ${type}`;
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#28a745' : '#dc3545'};
        color: white;
        padding: 15px 25px;
        border-radius: 50px;
        box-shadow: 0 10px 30px rgba(0,0,0,0.3);
        z-index: 9999;
        animation: slideInRight 0.3s ease-out;
    `;

    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'slideOutRight 0.3s ease-out';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}
