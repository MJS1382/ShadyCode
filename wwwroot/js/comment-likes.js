(() => {
  const setLikeState = (button, liked) => {
    const icon = button.querySelector("i");
    const label = button.querySelector("span");

    button.dataset.liked = String(liked);
    button.setAttribute("aria-label", liked ? "Unlike comment" : "Like comment");

    if (icon) {
      icon.classList.toggle("bi-hand-thumbs-up-fill", liked);
      icon.classList.toggle("bi-hand-thumbs-up", !liked);
    }

    if (label) {
      label.textContent = liked ? "Unlike" : "Like";
    }
  };

  const showSignInMessage = (status) => {
    if (!status) return;

    const loginLink = document.createElement("a");
    loginLink.href = "/auth/login";
    loginLink.textContent = "sign in";
    status.replaceChildren("Please ", loginLink, " to like this comment.");
  };

  document.addEventListener("click", async (event) => {
    if (!(event.target instanceof Element)) return;

    const button = event.target.closest("[data-comment-like]");
    if (!button || button.disabled) return;

    const commentId = button.dataset.commentId;
    if (!commentId) return;

    const card = button.closest(".comment-card");
    const count = card?.querySelector("[data-comment-like-count]");
    const status = card?.querySelector("[data-comment-like-status]");

    if (status) status.textContent = "";
    if (button.closest("[data-comments-loader]")?.dataset.authenticated === "false") {
      showSignInMessage(status);
      return;
    }

    button.disabled = true;

    try {
      const response = await fetch(
        `/api/blog/comments/like/${encodeURIComponent(commentId)}`,
        {
          method: "POST",
          headers: { Accept: "application/json" },
          credentials: "same-origin"
        }
      );

      const redirectedToLogin = response.redirected &&
        new URL(response.url, window.location.href).pathname === "/auth/login";
      if (response.status === 401 || redirectedToLogin) {
        showSignInMessage(status);
        return;
      }

      if (!response.ok) {
        throw new Error("Unable to update the comment like. Please try again.");
      }

      const result = await response.json();
      if (typeof result.isLikedByUser !== "boolean" ||
          !Number.isInteger(result.likeCount) ||
          result.likeCount < 0) {
        throw new Error("The server returned an invalid comment like response.");
      }

      setLikeState(button, result.isLikedByUser);
      if (count) count.textContent = result.likeCount.toString();
    } catch (error) {
      console.error("Failed to toggle the comment like", error);
      if (status) {
        status.textContent = error instanceof Error
          ? error.message
          : "Unable to update the comment like. Please try again.";
      }
    } finally {
      button.disabled = false;
    }
  });
})();
