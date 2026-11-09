(() => {
  const updateLikeButtonState = (button, liked) => {
    if (!button) return;

    const icon = button.querySelector("i");
    button.dataset.liked = String(liked);
    button.classList.toggle("liked", liked);

    if (icon) {
      icon.classList.toggle("bi-hand-thumbs-up-fill", liked);
      icon.classList.toggle("bi-hand-thumbs-up", !liked);
    }
  };

  const showSignInMessage = (status) => {
    if (!status) return;

    const loginLink = document.createElement("a");
    loginLink.href = "/auth/login";
    loginLink.textContent = "sign in";
    status.replaceChildren("Please ", loginLink, " to like this post.");
  };

  document.addEventListener("click", (event) => {
    if (!(event.target instanceof Element)) return;

    const likeButton = event.target.closest(".btn-like");
    if (!likeButton) return;

    likeButton.classList.remove("is-animating");
    void likeButton.offsetWidth;
    likeButton.classList.add("is-animating");
  });

  document.addEventListener("click", async (event) => {
    if (!(event.target instanceof Element)) return;

    const button = event.target.closest(".btn-like");
    if (!button || button.disabled) return;

    const status = button.closest(".like-btn-container")?.querySelector("[data-post-like-status]");
    if (status) status.textContent = "";

    if (button.dataset.authenticated === "false") {
      showSignInMessage(status);
      return;
    }

    button.disabled = true;
    try {
      const response = await fetch(`/api/blog/like/${button.dataset.postId}`);

      const redirectedToLogin = response.redirected &&
        new URL(response.url, window.location.href).pathname === "/auth/login";
      if (response.status === 401 || redirectedToLogin) {
        showSignInMessage(status);
        return;
      }

      if (!response.ok) {
        console.error("Failed to toggle the post like");
        console.error(await response.text());
        return;
      }

      const result = await response.json();
      const likeElement = document.getElementById("like-count");

      if (likeElement) {
        likeElement.textContent = result.likeCount.toString();
      }

      updateLikeButtonState(button, result.liked);
    } catch (error) {
      console.error("Failed to toggle the post like", error);
    } finally {
      button.disabled = false;
    }
  });
})();
