(() => {
  let scrollToTopButton = null;
  let articleEndMarker = null;
  let updateScrollToTopVisibility = null;

  const bindScrollToTopButton = () => {
    const button = document.querySelector("[data-scroll-to-top]");
    const marker = document.querySelector("[data-article-end]");

    if (button === scrollToTopButton && marker === articleEndMarker) {
      updateScrollToTopVisibility?.();
      return;
    }

    if (updateScrollToTopVisibility) {
      window.removeEventListener("scroll", updateScrollToTopVisibility);
      window.removeEventListener("resize", updateScrollToTopVisibility);
    }

    scrollToTopButton = button;
    articleEndMarker = marker;
    updateScrollToTopVisibility = null;

    if (!button || !marker) return;

    updateScrollToTopVisibility = () => {
      const isVisible = marker.getBoundingClientRect().top <= window.innerHeight;
      button.classList.toggle("is-visible", isVisible);
      button.setAttribute("aria-hidden", String(!isVisible));
      button.tabIndex = isVisible ? 0 : -1;
    };

    window.addEventListener("scroll", updateScrollToTopVisibility, { passive: true });
    window.addEventListener("resize", updateScrollToTopVisibility);
    button.addEventListener("click", () => {
      if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
        window.scrollTo(0, 0);
      } else {
        window.scrollTo({ top: 0, behavior: "smooth" });
      }
    });
    updateScrollToTopVisibility();
  };

  bindScrollToTopButton();
  new MutationObserver(bindScrollToTopButton).observe(document.body, { childList: true, subtree: true });
})();
