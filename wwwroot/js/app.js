const menuToggle = document.getElementById("menu-toggle");
const menu = document.getElementById("menu");

if (menuToggle && menu) {
  menuToggle.addEventListener("click", () => {
    const isOpen = menu.classList.toggle("open");
    menuToggle.classList.toggle("open", isOpen);
    console.log("menu is " + (isOpen ? "open" : "closed"));
  });
}

window.hideBootstrapModal = (selector) => {
    const element = document.querySelector(selector);

    if (!element) return;

    const modal = bootstrap.Modal.getOrCreateInstance(element);
    modal.hide();
};

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

  button.disabled = true;
  try {
    const response = await fetch(`/api/blog/like/${button.dataset.postId}`);

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