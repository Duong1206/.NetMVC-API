/**
 * Product Search & Filter Module
 * Provides real-time search, filtering, and pagination functionality
 */

class ProductSearchFilter {
  constructor() {
    this.currentFilters = {
      searchTerm: "",
      categoryId: null,
      coverTypeId: null,
      priceMin: null,
      priceMax: null,
      minRating: null,
      sortBy: "name",
      sortAscending: true,
      pageNumber: 1,
      pageSize: 12,
    };

    this.debounceTimer = null;
    this.isLoading = false;
    this.init();
  }

  init() {
    this.attachEventListeners();
    this.loadProductsFromUrl();
  }

  attachEventListeners() {
    // Search input with debounce
    const searchInput = document.getElementById("searchKeyword");
    if (searchInput) {
      searchInput.addEventListener("input", (e) => this.handleSearch(e));
    }

    // Category filter
    const categorySelect = document.getElementById("categoryFilter");
    if (categorySelect) {
      categorySelect.addEventListener("change", (e) =>
        this.handleCategoryFilter(e),
      );
    }

    // Price filter
    const priceMinInput = document.getElementById("priceMin");
    const priceMaxInput = document.getElementById("priceMax");
    if (priceMinInput)
      priceMinInput.addEventListener("change", () => this.handlePriceFilter());
    if (priceMaxInput)
      priceMaxInput.addEventListener("change", () => this.handlePriceFilter());

    // Sorting
    const sortSelect = document.getElementById("sortBy");
    if (sortSelect) {
      sortSelect.addEventListener("change", (e) => this.handleSort(e));
    }

    // Pagination
    document.addEventListener("click", (e) => {
      if (e.target.closest("[data-page]")) {
        const pageNum = e.target.dataset.page;
        this.goToPage(parseInt(pageNum));
      }
    });

    // Reset filters button
    const resetBtn = document.getElementById("resetFilters");
    if (resetBtn) {
      resetBtn.addEventListener("click", () => this.resetFilters());
    }
  }

  handleSearch(e) {
    clearTimeout(this.debounceTimer);

    const searchTerm = e.target.value.trim();
    this.currentFilters.searchTerm = searchTerm;
    this.currentFilters.pageNumber = 1;

    this.debounceTimer = setTimeout(() => {
      this.loadProducts();
    }, 300); // 300ms debounce
  }

  handleCategoryFilter(e) {
    this.currentFilters.categoryId = e.target.value
      ? parseInt(e.target.value)
      : null;
    this.currentFilters.pageNumber = 1;
    this.loadProducts();
  }

  handlePriceFilter() {
    const minInput = document.getElementById("priceMin");
    const maxInput = document.getElementById("priceMax");

    this.currentFilters.priceMin = minInput?.value
      ? parseFloat(minInput.value)
      : null;
    this.currentFilters.priceMax = maxInput?.value
      ? parseFloat(maxInput.value)
      : null;
    this.currentFilters.pageNumber = 1;

    this.loadProducts();
  }

  handleSort(e) {
    const sortValue = e.target.value;
    if (sortValue.startsWith("-")) {
      this.currentFilters.sortBy = sortValue.substring(1);
      this.currentFilters.sortAscending = false;
    } else {
      this.currentFilters.sortBy = sortValue;
      this.currentFilters.sortAscending = true;
    }
    this.currentFilters.pageNumber = 1;
    this.loadProducts();
  }

  goToPage(pageNumber) {
    this.currentFilters.pageNumber = pageNumber;
    this.loadProducts();
    this.scrollToTop();
  }

  resetFilters() {
    this.currentFilters = {
      searchTerm: "",
      categoryId: null,
      coverTypeId: null,
      priceMin: null,
      priceMax: null,
      minRating: null,
      sortBy: "name",
      sortAscending: true,
      pageNumber: 1,
      pageSize: 12,
    };

    // Reset form inputs
    const searchInput = document.getElementById("searchKeyword");
    if (searchInput) searchInput.value = "";

    const categorySelect = document.getElementById("categoryFilter");
    if (categorySelect) categorySelect.value = "";

    const priceMinInput = document.getElementById("priceMin");
    const priceMaxInput = document.getElementById("priceMax");
    if (priceMinInput) priceMinInput.value = "";
    if (priceMaxInput) priceMaxInput.value = "";

    const sortSelect = document.getElementById("sortBy");
    if (sortSelect) sortSelect.value = "name";

    this.loadProducts();
  }

  async loadProducts() {
    if (this.isLoading) return;

    this.isLoading = true;
    this.showSkeletonLoader();

    try {
      const params = new URLSearchParams();
      if (this.currentFilters.searchTerm)
        params.append("searchTerm", this.currentFilters.searchTerm);
      if (this.currentFilters.categoryId)
        params.append("categoryId", this.currentFilters.categoryId);
      if (this.currentFilters.priceMin)
        params.append("priceMin", this.currentFilters.priceMin);
      if (this.currentFilters.priceMax)
        params.append("priceMax", this.currentFilters.priceMax);
      params.append("sortBy", this.currentFilters.sortBy);
      params.append("sortAscending", this.currentFilters.sortAscending);
      params.append("pageNumber", this.currentFilters.pageNumber);
      params.append("pageSize", this.currentFilters.pageSize);

      // Update URL without reload
      const url = `/api/products?${params.toString()}`;
      window.history.replaceState(
        {},
        "",
        `${window.location.pathname}?${params.toString()}`,
      );

      const response = await fetch(url);
      if (!response.ok) throw new Error("Failed to load products");

      const data = await response.json();
      this.renderProducts(data);
      this.renderPagination(data);
    } catch (error) {
      console.error("Error loading products:", error);
      this.showErrorMessage("Failed to load products. Please try again.");
    } finally {
      this.isLoading = false;
    }
  }

  renderProducts(data) {
    const productList = document.getElementById("product-list");
    if (!productList) return;

    if (!data.items || data.items.length === 0) {
      productList.innerHTML =
        '<div class="col-12"><p class="text-center">No products found.</p></div>';
      return;
    }

    const html = data.items
      .map(
        (product) => `
            <div class="col-lg-4 col-md-6 col-sm-12 pb-1">
                <div class="card product-item border-0 mb-4 h-100">
                    <div class="card-header product-img position-relative overflow-hidden bg-transparent border p-0" style="aspect-ratio: 4/3;">
                        <a href="/customer/home/details/${product.id}">
                            <img class="img-fluid w-100" src="${product.imageUrl}" alt="${product.name}" style="object-fit: contain; width: 100%; height: 100%;">
                        </a>
                        <div class="btn-overlay">
                            <a href="/customer/cart/add?id=${product.id}" class="btn btn-sm btn-secondary m-2">
                                <i class="fa fa-shopping-cart"></i> Add to Cart
                            </a>
                        </div>
                    </div>
                    <div class="card-body border-left border-right text-center p-0 pt-4 pb-3">
                        <h6 class="mb-3">
                            <a href="/customer/home/details/${product.id}" class="text-dark">
                                ${this.highlightSearchTerm(product.name)}
                            </a>
                        </h6>
                        <div class="d-flex justify-content-center mb-2">
                            <small class="fa fa-star text-warning mr-1"></small>
                            <small>(${product.reviewCount} reviews)</small>
                        </div>
                        <h6 class="text-muted">₫${this.formatPrice(product.price)}</h6>
                    </div>
                    <div class="card-footer bg-light border">
                        <small class="text-muted">By ${product.author}</small><br>
                        <small class="text-muted">${product.categoryName}</small>
                    </div>
                </div>
            </div>
        `,
      )
      .join("");

    productList.innerHTML = html;
  }

  renderPagination(data) {
    const paginationContainer = document.getElementById("pagination");
    if (!paginationContainer) return;

    let html = '<nav><ul class="pagination justify-content-center">';

    // Previous button
    if (data.pageNumber > 1) {
      html += `<li class="page-item"><a class="page-link" href="#" data-page="${data.pageNumber - 1}">Previous</a></li>`;
    } else {
      html +=
        '<li class="page-item disabled"><span class="page-link">Previous</span></li>';
    }

    // Page numbers
    for (let i = 1; i <= data.totalPages; i++) {
      if (i === data.pageNumber) {
        html += `<li class="page-item active"><span class="page-link">${i}</span></li>`;
      } else {
        html += `<li class="page-item"><a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
      }
    }

    // Next button
    if (data.pageNumber < data.totalPages) {
      html += `<li class="page-item"><a class="page-link" href="#" data-page="${data.pageNumber + 1}">Next</a></li>`;
    } else {
      html +=
        '<li class="page-item disabled"><span class="page-link">Next</span></li>';
    }

    html += "</ul></nav>";
    paginationContainer.innerHTML = html;
  }

  showSkeletonLoader() {
    const productList = document.getElementById("product-list");
    if (!productList) return;

    const skeletonHtml = Array(12)
      .fill(
        `
            <div class="col-lg-4 col-md-6 col-sm-12 pb-1">
                <div class="card product-item border-0 mb-4 skeleton">
                    <div class="skeleton-header" style="aspect-ratio: 4/3;"></div>
                    <div class="card-body">
                        <div class="skeleton-text" style="height: 20px; margin-bottom: 10px;"></div>
                        <div class="skeleton-text" style="height: 15px; width: 80%;"></div>
                    </div>
                </div>
            </div>
        `,
      )
      .join("");

    productList.innerHTML = skeletonHtml;
  }

  highlightSearchTerm(text) {
    if (!this.currentFilters.searchTerm) return text;

    const regex = new RegExp(`(${this.currentFilters.searchTerm})`, "gi");
    return text.replace(regex, "<mark>$1</mark>");
  }

  formatPrice(price) {
    return price.toLocaleString("vi-VN");
  }

  showErrorMessage(message) {
    const productList = document.getElementById("product-list");
    if (productList) {
      productList.innerHTML = `<div class="col-12"><p class="text-center text-danger">${message}</p></div>`;
    }
  }

  loadProductsFromUrl() {
    const params = new URLSearchParams(window.location.search);

    if (params.has("searchTerm")) {
      this.currentFilters.searchTerm = params.get("searchTerm");
      const searchInput = document.getElementById("searchKeyword");
      if (searchInput) searchInput.value = this.currentFilters.searchTerm;
    }

    if (params.has("categoryId")) {
      this.currentFilters.categoryId = parseInt(params.get("categoryId"));
      const categorySelect = document.getElementById("categoryFilter");
      if (categorySelect) categorySelect.value = this.currentFilters.categoryId;
    }

    if (params.has("pageNumber")) {
      this.currentFilters.pageNumber = parseInt(params.get("pageNumber"));
    }

    this.loadProducts();
  }

  scrollToTop() {
    const productList = document.getElementById("product-list");
    if (productList) {
      productList.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }
}

// Initialize when DOM is ready
document.addEventListener("DOMContentLoaded", () => {
  window.productSearchFilter = new ProductSearchFilter();
});
