(() => {
  const highlightArticleCode = () => {
    const article = document.querySelector(".article-content");
    if (article && window.Prism) {
      window.Prism.highlightAllUnder(article);
    }
  };

  document.addEventListener("DOMContentLoaded", highlightArticleCode, { once: true });
  Blazor.addEventListener("enhancedload", highlightArticleCode);
})();