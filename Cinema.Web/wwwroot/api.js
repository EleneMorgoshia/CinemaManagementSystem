const config = window.CINEMAGO_CONFIG;
const roleClaim = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const idClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const nameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const emailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
const auth = {
  token: sessionStorage.getItem("cinemago.token"),
  user: null,
  role: null,
  clear() {
    this.token = this.user = this.role = null;
    sessionStorage.removeItem("cinemago.token");
  }
};
const path = value => encodeURIComponent(value);
function rolesOf(value) {
  const roles = value?.roles ?? value?.role ?? value;
  return (Array.isArray(roles) ? roles : typeof roles === "string" ? roles.split(",") : [])
    .map(role => String(role).trim());
}
function claimsOf(token) {
  const part = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
  return JSON.parse(new TextDecoder().decode(Uint8Array.from(atob(part), c => c.charCodeAt(0))));
}
async function api(route, method = "GET", body, anonymous = false) {
  const headers = { Accept: "application/json" };
  if (!anonymous && auth.token) headers.Authorization = "Bearer " + auth.token;
  if (body !== undefined) headers["Content-Type"] = "application/json";
  let response;
  try {
    response = await fetch(config.API_BASE_URL.replace(/\/$/, "") + "/api/" + route, {
      method, headers, body: body === undefined ? undefined : JSON.stringify(body)
    });
  } catch {
    throw new Error("Couldn’t reach CinemaGO’s API. Check the API address, HTTPS certificate, and allowed browser origin.");
  }
  const raw = await response.text();
  let data = raw;
  try { data = raw ? JSON.parse(raw) : null; } catch { /* Plain-text API response. */ }
  if (!response.ok) {
    const validation = data?.errors ? Object.values(data.errors).flat().join(" ") : "";
    const message = validation || data?.detail || data?.message || data?.title ||
      (typeof data === "string" && !data.trim().startsWith("<") && data.length < 700 ? data : "");
    const error = new Error(message || (response.status === 401 ? "Please check your credentials or sign in again." : `Request failed (${response.status}).`));
    error.status = response.status;
    throw error;
  }
  return data;
}
async function loadIdentity(token, email) {
  try {
    const claims = claimsOf(token);
    if (claims.exp && claims.exp * 1000 <= Date.now()) throw new Error("Your session has expired.");
    const id = claims[idClaim] ?? claims.nameid ?? claims.sub ?? claims.userId;
    if (!id) throw new Error("The sign-in token does not identify the account.");
    auth.token = token;
    const roles = rolesOf(claims[roleClaim] ?? claims.role ?? claims.roles ?? await api("User/GetUserRoleById/" + path(id)));
    auth.role = ["SuperAdmin", "Admin", "Customer"].find(role => roles.includes(role));
    if (!auth.role) throw new Error("Your account has no CinemaGO role.");
    const profile = await api("User/GetUser/" + path(id));
    auth.user = { ...profile, id, userName: profile?.userName ?? claims[nameClaim] ?? claims.unique_name ?? "CinemaGO member",
      email: profile?.email ?? claims[emailClaim] ?? claims.email ?? email ?? "" };
    sessionStorage.setItem("cinemago.token", token);
  } catch (error) {
    auth.clear();
    throw error;
  }
}
