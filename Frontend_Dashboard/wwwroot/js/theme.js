// --------------------------------------------------
// tiny helper – no dependency on Blazor
// --------------------------------------------------
const STORAGE_KEY = "preferred-theme";

/*  read user setting or system preference */
function getInitialTheme() {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) return saved;

    return window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "dark" : "light";
}

/*  apply + remember  */
export function setTheme(theme) {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem(STORAGE_KEY, theme);
}

/*  toggle helper for convenience */
export function toggleTheme() {
    const current = document.documentElement.getAttribute("data-theme") || "light";
    setTheme(current === "light" ? "dark" : "light");
}

export function getTheme() {
    return document.documentElement.getAttribute("data-theme") || "light";
}


/*  apply on first load */
setTheme(getInitialTheme());
