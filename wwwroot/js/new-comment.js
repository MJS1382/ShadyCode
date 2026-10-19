// AI GENERATED CODE BEGINS
(() => {
  const initializeForm = (form) => {
    if (form.dataset.newCommentInitialized === "true") return;

    const postId = form.dataset.postId;
    const username = form.dataset.username;
    const content = form.querySelector('[name="content"]');
    const submitButton = form.querySelector("[data-new-comment-submit]");
    const status = form.querySelector("[data-new-comment-status]");

    if (!postId || !username || !content || !submitButton || !status) {
      console.error("The new comment form is missing required configuration.");
      return;
    }

    form.dataset.newCommentInitialized = "true";
    let isSubmitting = false;
    form.addEventListener("submit", async (event) => {
      event.preventDefault();
      if (isSubmitting) return;

      const commentText = content.value.trim();
      status.textContent = "";
      status.classList.remove("text-danger", "text-success");

      if (!commentText) {
        content.setCustomValidity("Please enter a comment.");
        content.reportValidity();
        return;
      }

      content.setCustomValidity("");
      if (commentText.length > 512) {
        status.textContent = "Comments must be 512 characters or fewer.";
        status.classList.add("text-danger");
        return;
      }

      isSubmitting = true;
      submitButton.disabled = true;
      submitButton.textContent = "Posting…";

      try {
        const response = await fetch(`/api/blog/comment/${encodeURIComponent(postId)}`, {
          method: "POST",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json"
          },
          credentials: "same-origin",
          body: JSON.stringify({
            postId,
            username,
            content: commentText
          })
        });

        if (response.status === 401 || response.status === 403) {
          throw new Error("Please sign in to post a comment.");
        }
        if (!response.ok) {
          throw new Error(`Unable to post your comment (HTTP ${response.status}).`);
        }

        const comment = await response.json();
        if (!comment || typeof comment !== "object" || !comment.id) {
          throw new Error("The server returned an invalid comment response.");
        }

        document.dispatchEvent(new CustomEvent("blog:comment-created", {
          detail: { postId, comment }
        }));
        content.value = "";
        status.textContent = "Your comment was posted.";
        status.classList.add("text-success");
      } catch (error) {
        console.error("Failed to post comment", error);
        status.textContent = error instanceof Error
          ? error.message
          : "Unable to post your comment. Please try again.";
        status.classList.add("text-danger");
      } finally {
        isSubmitting = false;
        submitButton.disabled = false;
        submitButton.textContent = "Post comment";
      }
    });
  };

  const initializeForms = () => {
    document.querySelectorAll("[data-new-comment-form]").forEach(initializeForm);
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initializeForms, { once: true });
  } else {
    initializeForms();
  }

  new MutationObserver(initializeForms).observe(document.body, {
    childList: true,
    subtree: true
  });
})();
// AI GENERATED CODE ENDS
