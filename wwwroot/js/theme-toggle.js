(() => {
  const setTheme = (theme) => {
    const isDark = theme === "dark";
    document.body.setAttribute("data-bs-theme", isDark ? "dark" : "light");

    const themeToggle = document.getElementById("theme-toggle");
    if (!themeToggle) return;

    themeToggle.setAttribute("aria-label", `Switch to ${isDark ? "light" : "dark"} theme`);
    themeToggle.setAttribute("aria-pressed", String(isDark));

    const icon = themeToggle.querySelector("i");
    icon?.classList.toggle("bi-moon", isDark);
    icon?.classList.toggle("bi-sun", !isDark);
  };

  const restoreTheme = () => {
    const savedTheme = localStorage.getItem("preferred-theme");
    setTheme(savedTheme === "light" || savedTheme === "dark" ? savedTheme : "dark");
  };

  document.addEventListener("click", (event) => {
    if (!event.target?.closest?.("#theme-toggle")) return;

    const theme = document.body.getAttribute("data-bs-theme") === "dark" ? "light" : "dark";
    setTheme(theme);
    localStorage.setItem("preferred-theme", theme);
  });

  restoreTheme();
  Blazor.addEventListener("enhancedload", restoreTheme);
})();
