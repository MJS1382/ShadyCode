(() => {
  const pageSize = 10;

  const getCommentItems = (payload) => {
    if (!Array.isArray(payload)) {
      throw new Error("The server returned an invalid comments response.");
    }

    return payload.flatMap((item) => Array.isArray(item) ? item : [item]);
  };

  const formatDate = (value) => {
    if (!value) return "";

    const date = new Date(value);
    return Number.isNaN(date.getTime()) ? String(value) : date.toLocaleString();
  };

  const makeComment = (comment) => {
    const card = document.createElement("article");
    card.className = "card border-light shadow-sm comment-card mb-3";
    card.style.width = "100%";
    // AI GENERATED CODE BEGINS
    card.dataset.commentId = comment.id ?? "";
    // AI GENERATED CODE ENDS

    const body = document.createElement("div");
    body.className = "card-body";

    const heading = document.createElement("div");
    heading.className = "d-flex justify-content-between align-items-start mb-2";

    const authorDetails = document.createElement("div");
    authorDetails.className = "flex-grow-1";

    const author = document.createElement("h3");
    author.className = "card-subtitle mb-1 fw-bold text-primary h6";
    // AI GENERATED CODE BEGINS
    author.textContent = comment.user?.userName
      ?? comment.userName
      ?? (typeof comment.user === "string" ? comment.user : null)
      ?? "Anonymous";
    // AI GENERATED CODE ENDS

    const date = document.createElement("small");
    date.className = "text-muted d-block";
    date.textContent = formatDate(comment.date);

    const textContainer = document.createElement("div");
    textContainer.className = "mb-3";

    const text = document.createElement("p");
    text.className = "card-text";
    text.textContent = comment.text ?? "";

    const actions = document.createElement("div");
    actions.className = "d-flex align-items-center gap-2";

    const likeButton = document.createElement("button");
    // AI GENERATED CODE BEGINS
    const isLiked = comment.isLikedByUser === true;
    likeButton.className = `btn btn-sm ${isLiked ? "btn-primary" : "btn-outline-primary"}`;
    // AI GENERATED CODE ENDS
    likeButton.type = "button";
    likeButton.dataset.commentLike = "";
    likeButton.dataset.commentId = comment.id ?? "";
    // AI GENERATED CODE BEGINS
    likeButton.dataset.liked = String(isLiked);
    likeButton.setAttribute("aria-label", isLiked ? "Unlike comment" : "Like comment");
    // AI GENERATED CODE ENDS
    likeButton.disabled = !comment.id;

    const icon = document.createElement("i");
    // AI GENERATED CODE BEGINS
    icon.className = `bi ${isLiked ? "bi-hand-thumbs-up-fill" : "bi-hand-thumbs-up"}`;
    // AI GENERATED CODE ENDS
    icon.setAttribute("aria-hidden", "true");

    const label = document.createElement("span");
    // AI GENERATED CODE BEGINS
    label.textContent = isLiked ? "Unlike" : "Like";
    // AI GENERATED CODE ENDS
    likeButton.append(icon, label);

    const count = document.createElement("span");
    count.className = "badge bg-primary";
    count.dataset.commentLikeCount = "";
    count.textContent = Number.isInteger(comment.likes) ? String(comment.likes) : "0";

    const likeStatus = document.createElement("span");
    likeStatus.className = "small text-danger";
    likeStatus.dataset.commentLikeStatus = "";
    likeStatus.setAttribute("role", "status");
    likeStatus.setAttribute("aria-live", "polite");

    authorDetails.append(author, date);
    heading.append(authorDetails);
    textContainer.append(text);
    actions.append(likeButton, count, likeStatus);
    body.append(heading, textContainer, actions);
    card.append(body);

    return card;
  };

  const initializeLoader = (container) => {
    if (container.dataset.commentsInitialized === "true") return;

    const postId = container.dataset.postId;
    const list = container.querySelector("[data-comments-list]");
    const status = container.querySelector("[data-comments-status]");
    const moreButton = container.querySelector("[data-comments-more]");

    if (!postId || !list || !status || !moreButton) {
      console.error("The comments loader is missing required configuration.");
      return;
    }

    container.dataset.commentsInitialized = "true";
    const requestedPageSize = Number.parseInt(container.dataset.pageSize ?? "", 10);
    const currentPageSize = Number.isInteger(requestedPageSize) && requestedPageSize > 0
      ? requestedPageSize
      : pageSize;

    let page = 0;
    let isLoading = false;
    let hasLoaded = false;
    let finished = false;
    let bufferedComments = [];

    const loadNextPage = async () => {
      if (isLoading || finished) return;

      isLoading = true;
      moreButton.disabled = true;
      moreButton.hidden = true;
      status.textContent = "Loading comments…";
      status.classList.remove("text-danger");
      status.classList.add("text-muted");

      try {
        let comments;
        if (bufferedComments.length > 0) {
          comments = bufferedComments.splice(0, currentPageSize);
        } else {
          const response = await fetch(
            `/api/blog/comments/${encodeURIComponent(postId)}?page=${page}`,
            {
              headers: { Accept: "application/json" },
              credentials: "same-origin"
            }
          );

          if (!response.ok) {
            throw new Error(`Unable to load comments (HTTP ${response.status}).`);
          }

          const payload = await response.json();
          const items = getCommentItems(payload);

          // The endpoint may project the comments as a nested collection for each post.
          // Keep that collection paged in the UI instead of rendering every item at once.
          if (payload.some(Array.isArray)) {
            comments = items.splice(0, currentPageSize);
            bufferedComments = items;
          } else {
            comments = items;
          }
          page++;
        }

        for (const comment of comments) {
          if (comment && typeof comment === "object") {
            list.append(makeComment(comment));
          }
        }

        hasLoaded = true;
        finished = comments.length < currentPageSize && bufferedComments.length === 0;

        if (list.childElementCount === 0) {
          status.textContent = "No comments yet.";
          moreButton.hidden = true;
        } else if (finished) {
          status.textContent = "You’ve reached the end of the comments.";
          moreButton.hidden = true;
        } else {
          status.textContent = "";
          moreButton.hidden = false;
        }
      } catch (error) {
        console.error("Failed to load comments", error);
        status.textContent = error instanceof Error
          ? `${error.message} Try again.`
          : "Unable to load comments. Try again.";
        status.classList.remove("text-muted");
        status.classList.add("text-danger");
        moreButton.textContent = hasLoaded ? "Retry loading comments" : "Retry";
        moreButton.hidden = false;
      } finally {
        isLoading = false;
        moreButton.disabled = false;
      }
    };

    moreButton.addEventListener("click", () => {
      moreButton.textContent = "Load more comments";
      void loadNextPage();
    });

    if ("IntersectionObserver" in window) {
      const observer = new IntersectionObserver((entries) => {
        if (!entries.some((entry) => entry.isIntersecting)) return;
        observer.disconnect();
        void loadNextPage();
      }, { rootMargin: "300px 0px" });
      observer.observe(container);
    } else {
      void loadNextPage();
    }
  };

  const initializeLoaders = () => {
    document.querySelectorAll("[data-comments-loader]").forEach(initializeLoader);
  };

  // AI GENERATED CODE BEGINS
  document.addEventListener("blog:comment-created", (event) => {
    const { postId, comment } = event.detail ?? {};
    if (!postId || !comment || typeof comment !== "object") return;

    const loader = Array.from(document.querySelectorAll("[data-comments-loader]"))
      .find((element) => element.dataset.postId === postId);
    const list = loader?.querySelector("[data-comments-list]");
    const status = loader?.querySelector("[data-comments-status]");
    if (!list) return;

    if (comment.id && Array.from(list.querySelectorAll("[data-comment-id]"))
      .some((element) => element.dataset.commentId === String(comment.id))) {
      return;
    }

    list.prepend(makeComment(comment));
    if (status) {
      status.textContent = "";
      status.classList.remove("text-danger");
      status.classList.add("text-muted");
    }
  });
  // AI GENERATED CODE ENDS

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initializeLoaders, { once: true });
  } else {
    initializeLoaders();
  }

  new MutationObserver(initializeLoaders).observe(document.body, {
    childList: true,
    subtree: true
  });
})();
