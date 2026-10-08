window.hideBootstrapModal = (selector) => {
  const element = document.querySelector(selector);

  if (!element) return;

  const modal = bootstrap.Modal.getOrCreateInstance(element);
  modal.hide();
};
