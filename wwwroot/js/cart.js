// Professional Cart Management System
class CartManager {
    constructor() {
        this.toastContainer = null;
        this.init();
    }

    init() {
        this.createToastContainer();
        this.loadCartCount();
        this.bindEvents();
        this.setupAutoRefresh();
    }

    createToastContainer() {
        if (!this.toastContainer) {
            this.toastContainer = $('<div class="toast-container"></div>');
            $('body').append(this.toastContainer);
        }
    }

    bindEvents() {
        // Add to cart buttons
        $(document).on('click', '.btn-add-to-cart', (e) => {
            e.preventDefault();
            const button = $(e.currentTarget);
            const productId = button.data('product-id');
            const quantity = button.data('quantity') || 1;
            const variantId = button.data('variant-id');

            this.addToCart(productId, quantity, variantId, button);
        });

        // Remove from cart buttons
        $(document).on('click', '.btn-delete-product', (e) => {
            e.preventDefault();
            const button = $(e.currentTarget);
            const cartItemId = button.data('cart-item-id') || button.closest('article').data('cart-item-id');

            if (cartItemId) {
                this.removeFromCart(cartItemId, button);
            }
        });

        // Update quantity buttons
        $(document).on('click', '.btn-update-quantity', (e) => {
            e.preventDefault();
            const button = $(e.currentTarget);
            const cartItemId = button.data('cart-item-id');
            const quantity = parseInt(button.data('quantity'));

            this.updateCartItem(cartItemId, quantity, button);
        });
    }

    setupAutoRefresh() {
        // Refresh cart count every 30 seconds
        setInterval(() => {
            this.loadCartCount();
        }, 30000);
    }

    async loadCartCount() {
        try {
            const response = await fetch('/Cart/GetCartCount', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                }
            });

            if (response.ok) {
                const count = await response.json();
                this.updateCartCountDisplay(count);
            }
        } catch (error) {
            console.error('Cart count load error:', error);
        }
    }

    async loadCartPreview() {
        try {
            const response = await fetch('/Cart/GetCartItems', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                }
            });

            if (response.ok) {
                const data = await response.json();
                this.updateCartPreviewDisplay(data);
            }
        } catch (error) {
            console.error('Cart preview load error:', error);
        }
    }

    async addToCart(productId, quantity, variantId, button) {
        const originalText = button.html();
        const originalClass = button.attr('class');
        
        // Show loading state
        button.addClass('btn-loading').html('<i class="fa fa-spinner fa-spin"></i>');

        try {
            const formData = new FormData();
            formData.append('productId', productId);
            formData.append('quantity', quantity);
            if (variantId) {
                formData.append('variantId', variantId);
            }

            const response = await fetch('/Cart/AddToCart', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                },
                body: formData
            });

            const data = await response.json();

            if (response.ok && data.success) {
                // Update cart count with animation
                this.updateCartCountDisplay(data.cartCount);
                
                // Show professional success notification
                this.showToast({
                    type: 'success',
                    title: 'Sepete Eklendi!',
                    message: 'Ürün başarıyla sepete eklendi.',
                    duration: 3000
                });
                
                // Update cart preview
                await this.updateCartPreview();
                
                // Add visual feedback to button
                button.removeClass('btn-loading').html('<i class="fa fa-check"></i>');
                setTimeout(() => {
                    button.attr('class', originalClass).html(originalText);
                }, 1500);
                
            } else {
                this.showToast({
                    type: 'error',
                    title: 'Hata!',
                    message: data.message || 'Ürün sepete eklenemedi.',
                    duration: 4000
                });
                button.removeClass('btn-loading').attr('class', originalClass).html(originalText);
            }
        } catch (error) {
            console.error('Add to cart error:', error);
            this.showToast({
                type: 'error',
                title: 'Bağlantı Hatası!',
                message: 'Bir hata oluştu. Lütfen tekrar deneyin.',
                duration: 4000
            });
            button.removeClass('btn-loading').attr('class', originalClass).html(originalText);
        }
    }

    async removeFromCart(cartItemId, button) {
        const originalText = button.html();
        button.html('<i class="fa fa-spinner fa-spin"></i>');

        try {
            const formData = new FormData();
            formData.append('cartItemId', cartItemId);

            const response = await fetch('/Cart/RemoveItem', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                },
                body: formData
            });

            const data = await response.json();

            if (response.ok && data.success) {
                this.updateCartCountDisplay(data.cartCount);
                this.showMessage('Ürün sepetten kaldırıldı!', 'success');
                
                // Remove item from DOM
                button.closest('article').fadeOut(300, function() {
                    $(this).remove();
                });

                // Trigger cart count update
                this.loadCartCount();
            } else {
                this.showMessage(data.message || 'Ürün sepetten kaldırılamadı.', 'error');
            }
        } catch (error) {
            console.error('Remove from cart error:', error);
            this.showMessage('Bir hata oluştu. Lütfen tekrar deneyin.', 'error');
        } finally {
            button.html(originalText);
        }
    }

    async updateCartItem(cartItemId, quantity, button) {
        const originalText = button.html();
        button.html('<i class="fa fa-spinner fa-spin"></i>');

        try {
            const formData = new FormData();
            formData.append('cartItemId', cartItemId);
            formData.append('quantity', quantity);

            const response = await fetch('/Cart/UpdateQuantity', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                },
                body: formData
            });

            const data = await response.json();

            if (response.ok && data.success) {
                this.updateCartCountDisplay(data.cartCount);
                this.showMessage('Sepet güncellendi!', 'success');
                
                // Trigger cart count update
                this.loadCartPreview();
            } else {
                this.showMessage(data.message || 'Sepet güncellenemedi.', 'error');
            }
        } catch (error) {
            console.error('Update cart error:', error);
            this.showMessage('Bir hata oluştu. Lütfen tekrar deneyin.', 'error');
        } finally {
            button.html(originalText);
        }
    }

    updateCartCountDisplay(count) {
        // Update cart count in header with animation
        const cartCountElement = $('#cart-count');
        const cartButtonElement = $('.btn-my-cart .number');
        
        // Add animation class
        cartCountElement.addClass('cart-update-animation');
        cartButtonElement.addClass('cart-update-animation');
        
        // Update the count
        cartCountElement.text(count);
        cartButtonElement.text(count);
        
        // Update cart items count text
        const itemText = count === 1 ? 'ürün' : 'ürün';
        $('#cart-items-count').text(`${count} ${itemText}`);
        
        // Remove animation class after animation completes
        setTimeout(() => {
            cartCountElement.removeClass('cart-update-animation');
            cartButtonElement.removeClass('cart-update-animation');
        }, 600);
    }

    async updateCartPreview() {
        try {
            // Show loading state on cart dropdown
            const cartDropdown = $('.script-dropdown-1 .dropdown-menu');
            cartDropdown.addClass('cart-dropdown-updating');

            // Fetch updated cart data
            const response = await fetch('/Cart/GetCartPreview', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
                }
            });

            if (response.ok) {
                const data = await response.json();
                this.updateCartPreviewDisplay(data);
            }
        } catch (error) {
            console.error('Cart preview update error:', error);
        } finally {
            // Remove loading state
            setTimeout(() => {
                $('.script-dropdown-1 .dropdown-menu').removeClass('cart-dropdown-updating');
            }, 500);
        }
    }

    updateCartPreviewDisplay(data) {
        const cartPreviewList = $('#cart-preview-list');
        const cartTotal = $('#cart-total');
        
        // Update total with animation
        if (cartTotal.length) {
            cartTotal.fadeOut(200, function() {
                $(this).text(new Intl.NumberFormat('tr-TR', {
                    style: 'currency',
                    currency: 'TRY'
                }).format(data.totalAmount)).fadeIn(200);
            });
        }

        // Update items count
        const itemText = data.itemCount === 1 ? 'ürün' : 'ürün';
        $('#cart-items-count').text(`${data.itemCount} ${itemText}`);

        // Update items list if we have new data
        if (data.items && data.items.length > 0) {
            this.renderCartItems(data.items);
        } else if (data.itemCount === 0) {
            // Empty cart
            cartPreviewList.html(`
                <div class="text-center" style="padding: 20px;">
                    <i class="fa fa-shopping-cart" style="font-size: 48px; color: #ccc; margin-bottom: 15px;"></i>
                    <p style="color: #666; margin: 0;">Sepetinizde ürün bulunmuyor.</p>
                </div>
            `);
        }
    }

    renderCartItems(items) {
        const cartPreviewList = $('#cart-preview-list');
        
        if (items.length === 0) {
            cartPreviewList.html(`
                <div class="text-center" style="padding: 20px;">
                    <i class="fa fa-shopping-cart" style="font-size: 48px; color: #ccc; margin-bottom: 15px;"></i>
                    <p style="color: #666; margin: 0;">Sepetinizde ürün bulunmuyor.</p>
                </div>
            `);
            return;
        }

        let itemsHtml = '';
        items.slice(0, 3).forEach(item => {
            const mainImage = item.productImage || '/images/demo/demo_80x100.png';
            const imagePath = mainImage.startsWith('/') ? mainImage : '/' + mainImage;
            
            itemsHtml += `
                <article class="item post cart-item-added">
                    <div class="item-inner">
                        <div class="mv-dp-table align-top">
                            <div class="mv-dp-table-cell block-39-thumb">
                                <div class="thumb-inner mv-lightbox-style-1">
                                    <a href="/Product/Details/${item.productSlug}" title="${item.productName}">
                                        <img src="${imagePath}" alt="${item.productName}" class="block-39-thumb-img" />
                                    </a>
                                </div>
                            </div>
                            <div class="mv-dp-table-cell block-39-main">
                                <div class="block-39-main-inner">
                                    <div class="block-39-title">
                                        <a href="/Product/Details/${item.productSlug}" title="${item.productName}" class="mv-overflow-ellipsis">${item.productName}</a>
                                    </div>
                                    <div class="block-39-price">
                                        <div class="new-price">${new Intl.NumberFormat('tr-TR', {
                                            style: 'currency',
                                            currency: 'TRY'
                                        }).format(item.productPrice)}</div>
                                    </div>
                                    <ul class="block-24-meta mv-ul">
                                        <li class="meta-item mv-icon-left-style-3"><span class="text">Adet: ${item.quantity}</span></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <button type="button" title="Sepetten Kaldır" class="mv-btn mv-btn-style-4 fa fa-close btn-delete-product" data-product-id="${item.productId}"></button>
                    </div>
                </article>
            `;
        });

        cartPreviewList.html(itemsHtml);
    }

    showToast(options) {
        const {
            type = 'info',
            title = '',
            message = '',
            duration = 4000
        } = options;

        const icons = {
            success: 'fa-check-circle',
            error: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };

        const toast = $(`
            <div class="toast toast-${type}">
                <div class="toast-icon">
                    <i class="fa ${icons[type]}"></i>
                </div>
                <div class="toast-content">
                    ${title ? `<div class="toast-title">${title}</div>` : ''}
                    <div class="toast-message">${message}</div>
                </div>
                <button class="toast-close" type="button">
                    <i class="fa fa-times"></i>
                </button>
                <div class="toast-progress"></div>
            </div>
        `);

        // Add to container
        this.toastContainer.append(toast);

        // Auto remove
        setTimeout(() => {
            this.removeToast(toast);
        }, duration);

        // Manual close
        toast.find('.toast-close').on('click', () => {
            this.removeToast(toast);
        });

        return toast;
    }

    removeToast(toast) {
        toast.addClass('slideOutRight');
        setTimeout(() => {
            toast.remove();
        }, 300);
    }

    // Legacy method for backward compatibility
    showMessage(message, type = 'info') {
        this.showToast({
            type: type,
            message: message,
            duration: 3000
        });
    }
}

// Initialize cart manager when document is ready
$(document).ready(function() {
    window.cartManager = new CartManager();
});

// Export for global access
window.CartManager = CartManager;
