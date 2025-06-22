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

export function getTheme() {
    return document.documentElement.getAttribute("data-theme") || "light";
}

/*  toggle helper for convenience */
export function toggleTheme() {
    const next = getTheme() === "dark" ? "light" : "dark";
    setTheme(next);
    return next;                
}

setTheme(getInitialTheme());
