const $ = (selector, root = document) => root.querySelector(selector);
const main = $("#main");
const dialog = $("#dialog");
const esc = value => String(value ?? "").replace(/[&<>"']/g, c => ({ "&":"&amp;", "<":"&lt;", ">":"&gt;", '"':"&quot;", "'":"&#39;" })[c]);
const arr = value => Array.isArray(value) ? value : [];
const same = (a, b) => String(a) === String(b);
const money = value => config.CURRENCY + Number(value).toLocaleString("en", { maximumFractionDigits: 2 });
const date = value => new Date(value);
const starts = s => s.startsAt ?? s.sessionStartsAt ?? s.startTime;
const dayKey = value => {
  const d = date(value);
  return [d.getFullYear(), String(d.getMonth()+1).padStart(2,"0"), String(d.getDate()).padStart(2,"0")].join("-");
};
const time = value => date(value).toLocaleTimeString("en-GB", {hour:"2-digit",minute:"2-digit"});
const fullDate = value => date(value).toLocaleDateString("en-GB", {weekday:"short",day:"numeric",month:"short",year:"numeric"});
const age = value => config.AGE_RATINGS[value] ?? value ?? "NR";
const staff = () => ["Admin", "SuperAdmin"].includes(auth.role);
const movieKeys = {
  "lord of the rings":"rings", "the lord of the rings":"rings", "interstellar":"interstellar",
  "the dark knight":"dark-knight", "inception":"inception", "jurassic park":"jurassic-park",
  "back to the future":"back-to-the-future", "finding nemo":"finding-nemo", "dunkirk":"dunkirk"
};
const fallback = "assets/movies/placeholder.svg";
function artwork(title, wide = false) {
  const key = movieKeys[String(title ?? "").trim().toLowerCase().replace(/\s+/g," ")];
  return key ? "assets/movies/" + key + (wide ? "-wide" : "") + ".jpg" : fallback;
}
const poster = (title, extra = "") => `<img ${extra} src="${artwork(title)}" alt="${esc(title)} poster">`;
document.addEventListener("error", event => {
  if (event.target.matches?.("img") && event.target.getAttribute("src") !== fallback) event.target.src = fallback;
}, true);
const icons = {
  sun:'<circle cx="12" cy="12" r="4"/><path d="M12 2v2m0 16v2M2 12h2m16 0h2M5 5l1.5 1.5m11 11L19 19M5 19l1.5-1.5m11-11L19 5"/>',
  moon:'<path d="M21 13A9 9 0 1 1 11 3a7 7 0 0 0 10 10Z"/>',
  Instagram:'<rect x="3" y="3" width="18" height="18" rx="5"/><circle cx="12" cy="12" r="4"/><path d="M17 7h.01"/>',
  TikTok:'<path d="M14 3v12a4 4 0 1 1-4-4m4-8c1 4 3 5 7 6"/>',
  Facebook:'<path d="M14 22v-9h3l1-4h-4V7c0-1 1-2 2-2h2V2h-3a5 5 0 0 0-5 5v2H7v4h3v9"/>',
  X:'<path d="m4 3 13 18h4L8 3H4Zm16 0L4 21"/>'
};
const icon = name => `<svg aria-hidden="true" viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round">${icons[name]}</svg>`;
const logo = '<a class="logo" href="#home" aria-label="CinemaGO home"><img src="assets/mark.svg" alt="">Cinema<span>GO</span></a>';
const button = (label, action, style = "", disabled = false) =>
  `<button type="button" class="btn ${style}" data-action="${esc(action)}" ${disabled ? "disabled" : ""}>${esc(label)}</button>`;
const empty = message => `<div class="empty">${esc(message)}</div>`;
const field = (label, name, value = "", type = "text", attrs = "") =>
  `<label>${esc(label)}<input name="${name}" type="${type}" value="${esc(value)}" required ${attrs}></label>`;
const options = (items, selected, first = "") => (first ? `<option value="">${esc(first)}</option>` : "") +
  items.map(([value, label]) => `<option value="${esc(value)}" ${same(value,selected) ? "selected" : ""}>${esc(label)}</option>`).join("");
const sectionHead = (tag, title, actions = "") => `<div class="section-head"><div><p class="eyebrow">${tag}</p><h2>${title}</h2></div><div class="actions">${actions}</div></div>`;
let movies = [], genres = [], sessions = [];
let currentPage = "home", renderId = 0, resume = null, movieSearch = "", movieGenre = "", heroIndex = 0, heroTimer;
let scheduleFilter = { day:"", hall:"", time:"", genre:"" };
let bookingTab = "active";
const activeFor = id => sessions.filter(s => same(s.movieId, id));
const movie = id => movies.find(m => same(m.id,id));
const genreIds = m => arr(m.genreIds ?? m.genres ?? m.movieGenreRecords).map(g => g?.genreId ?? g?.id ?? g);
const genreNames = m => genreIds(m).map(id => genres.find(g => same(g.id,id))?.genreName).filter(Boolean).join(" · ");
const sessionTitle = s => s.movieTitle ?? movie(s.movieId)?.title ?? "Movie";
const sessionLabel = s => `${fullDate(starts(s))} · ${time(starts(s))} · ${s.hallName ?? "Hall " + s.hallId}`;
function notify(message) {
  $("#notice").textContent = message;
  clearTimeout(notify.timer);
  notify.timer = setTimeout(() => { $("#notice").textContent = ""; }, 4200);
}
function header() {
  $("#footer").classList.toggle("customer-footer", !staff());
  const active = currentPage === "movie" ? "movies" : currentPage;
  $("#header").innerHTML = `<div class="wrap topbar">${logo}<nav aria-label="Main navigation">${[["home","Home"],["movies","Movies"],["profile","My Profile"]].map(([p,l]) =>
    `<a href="#${p}" ${p===active ? 'aria-current="page"' : ""}>${l}</a>`).join("")}</nav>
    <div class="actions"><button type="button" class="icon-btn" data-action="theme" aria-label="Toggle light or dark mode">${icon(document.documentElement.dataset.theme === "dark" ? "sun" : "moon")}</button>
    ${auth.user ? `<a class="avatar" href="#profile" aria-label="My Profile">${esc(auth.user.userName.slice(0,1).toUpperCase())}</a>` : button("Log In","login","primary")}</div></div>`;
}
function footer() {
  $("#footer").innerHTML = `<div class="wrap foot"><div>${logo}<p>Good films. Great seats.</p></div><div class="socials">${Object.entries(config.SOCIAL).map(([name,url]) =>
    `<a class="icon-btn" aria-label="${name}" href="${esc(url)}" target="_blank" rel="noopener noreferrer">${icon(name)}</a>`).join("")}</div><small>© ${new Date().getFullYear()} CinemaGO. All rights reserved.</small></div>`;
}
function openDialog(title, content, label = "") {
  dialog.innerHTML = `<div class="dialog-head"><div><p class="eyebrow">${esc(label || "CinemaGO")}</p><h2 id="dialog-title">${esc(title)}</h2></div>${button("×","close","close")}</div>${content}`;
  if (!dialog.open) dialog.showModal();
}
function bindForm(form, task) {
  form.addEventListener("submit", async event => {
    event.preventDefault();
    const submit = form.querySelector('[type="submit"]');
    if (submit.disabled) return;
    submit.disabled = true;
    form.dataset.saving = "true";
    form.querySelector(".error").textContent = "";
    try { await task(Object.fromEntries(new FormData(form)), new FormData(form)); }
    catch (error) { form.querySelector(".error").textContent = error.message; }
    finally { submit.disabled = false; delete form.dataset.saving; }
  });
}
function formBody(fields, submit, cancel = "close") {
  return `<form><div class="fields">${fields}</div><p class="error" role="alert"></p><div class="actions form-actions">${cancel ? button("Cancel",cancel) : ""}<button type="submit" class="btn primary">${esc(submit)}</button></div></form>`;
}
function confirmAction(title, message, label, task) {
  openDialog(title, `<p>${esc(message)}</p>` + formBody("",label), "Confirmation");
  bindForm($("form",dialog), async () => { await task(); dialog.close(); notify("Changes saved."); });
}
function loginPanel(register = false, message = "", email = "") {
  openDialog(register ? "Create your account" : "Welcome back",
    `<div class="auth-panel">` + (message ? `<p class="muted">${esc(message)}</p>` : "") +
    formBody((register ? field("Username","userName") : "") + field("Email","email",email,"email",'autocomplete="email"') +
      field("Password","password","","password",register ? 'autocomplete="new-password"' : 'autocomplete="current-password"') +
      (register ? field("Confirm Password","confirmPassword","","password",'autocomplete="new-password"') : ""),
      register ? "Create Account" : "Log In", "") +
    `<div class="auth-switch">${button(register ? "Back to Log In" : "Create Account",register ? "login" : "register")}</div></div>`);
  bindForm($("form",dialog), async data => {
    if (register) {
      if (data.password !== data.confirmPassword) throw new Error("The passwords must match.");
      await api("Auth/Register","POST",data,true);
      loginPanel(false,"Account created. Sign in to continue.",data.email);
      return;
    }
    const result = await api("Auth/Login","POST",data,true);
    const token = result?.token ?? result?.accessToken ?? result?.jwtToken ?? result?.Token ?? result;
    if (typeof token !== "string" || token.split(".").length !== 3) throw new Error("The API did not return a usable sign-in token.");
    await loadIdentity(token, data.email);
    const next = resume;
    resume = null;
    dialog.close();
    await render();
    if (next) await next();
  });
}
function requireLogin(task) {
  if (auth.user) return task();
  resume = task;
  loginPanel(false,"Sign in to continue where you left off.");
}
async function catalog() {
  const [m,g,s] = await Promise.all([api("Movie/GetAllMovies"),api("Genre/GetAllGenres"),api("Session/GetActiveSessions")]);
  movies = arr(m); genres = arr(g); sessions = arr(s).sort((a,b) => date(starts(a))-date(starts(b)));
}
function cards(items) {
  return items.length ? `<div class="movie-grid">${items.map(m => {
    const shows = activeFor(m.id);
    const count = shows.length;
    return `<article class="movie-card"><a class="poster" href="#movie/${path(m.id)}" aria-label="View ${esc(m.title)}">${poster(m.title,'loading="lazy"')}<span class="rating">${esc(age(m.ageRating))}</span></a>
      <div class="card-body"><h3><a href="#movie/${path(m.id)}">${esc(m.title)}</a></h3>
      <p class="genre-line">${esc(genreNames(m) || "CinemaGO catalog")}</p>
      ${staff() ? `<span class="badge ${count ? "good" : ""}">${count ? count + " active showtime" + (count===1?"":"s") : "No active showtimes"}</span>` : ""}
      <div class="card-bottom"><span>${count ? "From <strong>" + money(Math.min(...shows.map(s => s.standardPrice))) + "</strong>" : staff() ? "" : "No showtimes available"}</span>
      ${staff() ? (count ? "" : button("Schedule Showtime","schedule:" + m.id,"small")) : button("Book Now","book:" + m.id,"primary small",!count)}</div>
      ${staff() ? `<div class="actions staff-actions">${button("Edit","editMovie:"+m.id,"small")}${button("Delete","deleteMovie:"+m.id,"small danger")}</div>` : ""}</div></article>`;
  }).join("")}</div>` : empty("No movies match your search.");
}
function hero() {
  const featured = [...new Map(sessions.map(s => [String(s.movieId),s])).values()];
  const m = featured[heroIndex % (featured.length || 1)];
  return `<section class="hero">${m ? `<a class="hero-image" href="#movie/${path(m.movieId)}" data-action="book:${m.movieId}" aria-label="Book ${esc(sessionTitle(m))}"><img alt="" src="${artwork(sessionTitle(m),true)}"></a>` : ""}<div class="hero-content">
    <p class="eyebrow"><span class="live-dot"></span> ${m ? "Now showing" : "The CinemaGO experience"}</p>
    <h1>${m ? esc(sessionTitle(m)) : "Your next<br>movie night."}</h1><p>${m ? esc(movie(m.movieId)?.description || "See it on the big screen.") : "Find your next favorite film and make a night of it."}</p>
    ${m ? button("Book Now","book:"+m.movieId,"primary") : '<a class="btn primary" href="#movies">Explore Movies</a>'}
    </div><div class="hero-controls">${featured.length > 1 ? button("‹","heroPrev","glass") + `<span>${heroIndex % featured.length + 1} / ${featured.length}</span>` + button("›","heroNext","glass") : ""}</div></section>`;
}
function refreshHero(step) {
  const count = new Set(sessions.map(s => String(s.movieId))).size;
  if (!count) return;
  heroIndex = (heroIndex + step + count) % count;
  const root = $("#hero-root");
  const focused = document.activeElement?.dataset.action;
  if (root) { root.innerHTML = hero(); if (focused?.startsWith("hero")) root.querySelector(`[data-action="${focused}"]`)?.focus(); }
}
function showtimes() {
  const root = $("#showtimes");
  if (!root) return;
  const days = [...new Set(sessions.map(s => dayKey(starts(s))))];
  if (!days.includes(scheduleFilter.day)) scheduleFilter.day = days[0] ?? "";
  const daySessions = sessions.filter(s => dayKey(starts(s)) === scheduleFilter.day);
  const halls = [...new Map(daySessions.map(s => [String(s.hallId),s.hallName ?? "Hall "+s.hallId])).entries()];
  if (!halls.some(([id]) => id === scheduleFilter.hall)) scheduleFilter.hall = "";
  const matching = daySessions.filter(s => (!scheduleFilter.hall || same(s.hallId,scheduleFilter.hall)) &&
    (!scheduleFilter.time || (date(starts(s)).getHours() < 17 ? "early" : "late") === scheduleFilter.time) &&
    (!scheduleFilter.genre || genreIds(movie(s.movieId) ?? {}).some(g => same(g,scheduleFilter.genre))));
  const grouped = Map.groupBy ? Map.groupBy(matching,s => s.movieId) : matching.reduce((map,s) => map.set(s.movieId,[...(map.get(s.movieId) || []),s]),new Map());
  root.innerHTML = sectionHead("Plan your visit","Showtimes") + `<div class="schedule">
    <div class="schedule-top"><div class="date-picker">${button("‹","datesBack","small")}<div class="scroller" id="dates">${days.map(d => {
      const obj = date(d+"T12:00:00"); return button(obj.toLocaleDateString("en",{weekday:"short"}) + " " + obj.getDate(),"day:"+d,d===scheduleFilter.day?"primary":"");
    }).join("")}</div>${button("›","datesNext","small")}</div>
    <label>Time<select id="show-time">${options([["early","Before 17:00"],["late","17:00 and later"]],scheduleFilter.time,"All Times")}</select></label>
    <label>Genre<select id="show-genre">${options(genres.map(g=>[g.id,g.genreName]),scheduleFilter.genre,"All Genres")}</select></label></div>
    <div class="scroller halls">${[["","All Halls"],...halls].map(([id,label])=>button(label,"hallFilter:"+id,id===scheduleFilter.hall?"primary":"")).join("")}</div>
    ${matching.length ? `<div class="showtime-results">${[...grouped.values()].map(showtimeCard).join("")}</div>` : empty("No active showtimes match your filters.")}</div>`;
  const scroller = $("#dates");
  const selected = $(".primary",scroller);
  if (selected) scroller.scrollLeft = Math.max(0, selected.getBoundingClientRect().left - scroller.getBoundingClientRect().left - 4);
}
function showtimeCard(group) {
  const first = group[0];
  const details = movie(first.movieId);
  const title = details?.title ?? sessionTitle(first);
  const link = "#movie/" + path(first.movieId);
  const duration = details?.durationMinutes ?? first.durationMinutes;
  const metadata = [age(details?.ageRating ?? first.ageRating),
    duration ? duration + " min" : "", genreNames(details ?? {})].filter(Boolean).join(" · ");
  return `<article class="showtime-card">
    <a class="showtime-poster" href="${link}">${poster(title,'loading="lazy"')}</a>
    <div class="showtime-content"><h3><a href="${link}">${esc(title)}</a></h3>
      <p class="muted">${esc(metadata)}</p>
      ${details?.description ? `<p class="showtime-description">${esc(details.description)}</p>` : ""}
      <div class="showtime-screenings">${group.map(s => `<div class="screening">
        ${button(time(starts(s)) + " · " + (s.hallName ?? "Hall " + s.hallId),"seat:"+s.id)}
        <small>Standard ${money(s.standardPrice)} · VIP ${money(s.vipPrice)}</small>
      </div>`).join("")}</div>
    </div></article>`;
}
function filters() {
  return `<div class="filters"><label>Search by Title<input id="movie-search" type="search" placeholder="Find a movie…" value="${esc(movieSearch)}"></label>
    <label>Genre<select id="movie-genre">${options(genres.map(g=>[g.id,g.genreName]),movieGenre,"All Genres")}</select></label></div>`;
}
function filterMovies() {
  $("#movie-grid").innerHTML = cards(movies.filter(m => String(m.title).toLowerCase().includes(movieSearch.trim().toLowerCase()) &&
    (!movieGenre || genreIds(m).some(id => same(id,movieGenre)))));
}
function showList(list, book = true) {
  return list.length ? list.map(s => `<article class="show-row"><div><strong>${esc(sessionLabel(s))}</strong><p>Standard ${money(s.standardPrice)} · VIP ${money(s.vipPrice)}</p></div>${book ? button("Choose Seats","seat:"+s.id,"primary") : ""}</article>`).join("") : empty(staff() ? "No active showtimes." : "No showtimes available.");
}
async function render() {
  const version = ++renderId;
  clearInterval(heroTimer);
  const [route, id] = (location.hash.slice(1) || "home").split("/");
  if (route === "main") return;
  currentPage = ["home","movies","movie","profile"].includes(route) ? route : "home";
  header();
  main.innerHTML = `<div class="wrap pad" aria-busy="true">${empty("Loading CinemaGO…")}</div>`;
  try {
    if (currentPage === "profile") {
      if (!auth.user) {
        main.innerHTML = `<div class="wrap pad">${sectionHead("Your account","My Profile",button("Log In","login","primary"))}${empty("Sign in to see your account and bookings.")}</div>`;
        if (!dialog.open) loginPanel();
        return;
      }
      await profile(version);
      return;
    }
    if (currentPage === "home" && staff()) {
      operationsHome(version);
      return;
    }
    await catalog();
    if (version !== renderId) return;
    if (currentPage === "home") {
      main.innerHTML = `<div class="wrap pad"><div id="hero-root">${hero()}</div>
      <section class="page-section">${sectionHead("The big screen","Explore the Movies",'<a class="btn" href="#movies">All Movies →</a>')}${cards(movies)}</section>
      <section id="showtimes" class="page-section"></section></div>`;
      showtimes();
      heroTimer = setInterval(() => { if (!dialog.open && !matchMedia("(prefers-reduced-motion: reduce)").matches && !$("#hero-root")?.matches(":focus-within, :hover")) refreshHero(1); }, 8000);
    } else if (currentPage === "movies") {
      if (movieGenre && !genres.some(g=>same(g.id,movieGenre))) movieGenre = "";
      main.innerHTML = `<div class="wrap pad">${sectionHead("CinemaGO catalog","Movies",staff() ? button("Manage Genres","genres") + button("+ Add Movie","addMovie","primary") : "")}
      <div id="movie-form"></div><div id="movie-filters">${filters()}</div><div id="movie-grid"></div></div>`;
      filterMovies();
    } else {
      const m = await api("Movie/GetMovie/"+path(decodeURIComponent(id ?? "")));
      if (version !== renderId) return;
      main.innerHTML = `<div class="wrap pad"><a class="back" href="#movies">← Back to Movies</a><section class="movie-detail">${poster(m.title)}<div>
      <p class="eyebrow">On the big screen</p><h1>${esc(m.title)}</h1><p class="detail-meta"><span class="badge">${esc(age(m.ageRating))}</span> ${esc(m.durationMinutes)} min · ${esc(genreNames(m))}</p>
      <p class="description">${esc(m.description)}</p><h2>Active Showtimes</h2>${showList(activeFor(m.id),!staff())}</div></section></div>`;
    }
  } catch (error) {
    if (version === renderId) main.innerHTML = `<div class="wrap pad">${empty(error.message)}${button("Try Again","retry","primary")}</div>`;
  }
}
async function bookingStart(id) {
  const m = movie(id);
  const list = activeFor(id);
  openDialog(m?.title ?? "Choose a Showtime",showList(list),"Step 1 · Showtime");
}
async function seatsStart(id) {
  const s = sessions.find(session => same(session.id,id)) ?? await api("Session/GetSessionById/"+path(id));
  openDialog("Choose your seat",empty("Loading seats…"),"Step 2 · Seating");
  const loading = $(".empty",dialog);
  const [all,available] = await Promise.all([
    api("Seat/GetSeatsByHallId/"+path(s.hallId)),
    api("Seat/GetAvailableSeatsBySessionId/"+path(id))
  ]);
  const free = new Set(arr(available).map(seat => String(seat?.id ?? seat?.seatId ?? seat)));
  const rows = new Map();
  arr(all).sort((a,b)=>a.rowNumber-b.rowNumber || a.seatNumber-b.seatNumber).forEach(seat => {
    if (!rows.has(seat.rowNumber)) rows.set(seat.rowNumber,[]);
    rows.get(seat.rowNumber).push(seat);
  });
  const grid = [...rows].map(([row,list]) => `<div class="seat-row"><strong>Row ${esc(row)}</strong>${list.map(seat =>
    `<button type="button" class="seat ${seat.isVip?"vip":""}" data-seat="${esc(seat.id)}" aria-pressed="false" aria-label="Row ${esc(row)}, seat ${esc(seat.seatNumber)}, ${seat.isVip?"VIP":"Standard"}${free.has(String(seat.id))?"":", occupied"}" ${free.has(String(seat.id))?"":"disabled"}>${esc(seat.seatNumber)}</button>`).join("")}</div>`).join("");
  if (!dialog.open || !dialog.contains(loading)) return;
  openDialog("Choose your seat", `<p><strong>${esc(sessionTitle(s))}</strong><br>${esc(sessionLabel(s))}</p><div class="legend">
  <span>Standard ${money(s.standardPrice)}</span><span>VIP ${money(s.vipPrice)}</span><span>Occupied</span><span>Selected</span></div>
  <div class="screen">SCREEN</div><div class="seat-grid">${grid || empty("This hall has no seats.")}</div>
  ${free.size ? "" : empty("No seats are available for this showtime.")}<p id="seat-summary" aria-live="polite">Select one available seat.</p>`+
    formBody("","Confirm Booking"),"Step 2 · Seating");
  let selected;
  const form = $("form",dialog);
  const submit = $('button[type="submit"]',form);
  submit.disabled = true;
  $(".seat-grid",dialog).addEventListener("click", event => {
    const control = event.target.closest("[data-seat]");
    if (!control || control.disabled || form.dataset.saving) return;
    selected = arr(all).find(seat=>same(seat.id,control.dataset.seat));
    dialog.querySelectorAll("[data-seat]").forEach(b=>b.setAttribute("aria-pressed",String(b===control)));
    $("#seat-summary",dialog).textContent = `Row ${selected.rowNumber} · Seat ${selected.seatNumber} · ${selected.isVip?"VIP":"Standard"} · ${money(selected.isVip?s.vipPrice:s.standardPrice)}`;
    submit.disabled = false;
  });
  bindForm(form, async () => {
    const booking = await api("Booking/AddBooking","POST",{sessionId:Number(id),seatId:Number(selected.id)});
    const bookingId = booking?.id ?? booking?.bookingId;
    let ticket = booking?.ticket ?? booking;
    if (!ticket?.ticketCode && bookingId) {
      try { ticket = await api("Ticket/GetTicketByBookingId/"+path(bookingId)); } catch { ticket = null; }
    }
    openDialog("You’re booked!",`<p>${esc(sessionTitle(s))} · Row ${esc(selected.rowNumber)}, Seat ${esc(selected.seatNumber)}</p>` +
      (ticket?.ticketCode ? ticketView(ticket) : '<p>Your booking is confirmed. Check My Bookings for the ticket.</p>') +
      '<a class="btn primary" href="#profile" data-action="bookingsLink">View My Bookings</a>',"Booking confirmed");
  });
}
const ticketView = ticket => `<div class="ticket"><p class="eyebrow">Ticket code</p><strong>${esc(ticket.ticketCode)}</strong>${ticket.finalPrice == null ? "" : `<p>Final price ${money(ticket.finalPrice)}</p>`}</div>`;
async function ticketByBooking(id) {
  const t = await api("Ticket/GetTicketByBookingId/"+path(id));
  openDialog("Your Ticket",t?.ticketCode ? ticketView(t) : empty("Ticket unavailable."));
}
async function enrichBookings(list) {
  const lookup = (ids, route) => Promise.all([...new Set(ids.filter(id=>id!=null))].map(async id => {
    try { return [String(id), await api(route + path(id))]; } catch { return [String(id),{}]; }
  })).then(entries=>new Map(entries));
  const [sessionMap,seatMap] = await Promise.all([
    lookup(list.filter(b=>!b.session && (!starts(b) || !b.movieTitle || !b.hallName)).map(b=>b.sessionId),"Session/GetSessionById/"),
    lookup(list.filter(b=>!b.seat && (b.rowNumber==null || b.seatNumber==null || b.isVip==null)).map(b=>b.seatId),"Seat/GetSeatById/")
  ]);
  return list.map(b => ({...b, displaySession:{...sessionMap.get(String(b.sessionId)),...b,...b.session},
    displaySeat:{...seatMap.get(String(b.seatId)),...b,...b.seat}}));
}
const bookingId = b => b.id ?? b.bookingId;
const isActive = b => b.status === true || b.status === 1 || /^active$/i.test(String(b.status));
const isUpcoming = b => isActive(b) && date(starts(b.displaySession)) > Date.now();
function bookingCard(b, own = false) {
  const s = b.displaySession, seat = b.displaySeat, id = bookingId(b);
  const title = sessionTitle(s), showDate = starts(s), hall = s.hallName ?? (s.hallId ? "Hall "+s.hallId : "Hall unavailable");
  const label = !isActive(b) ? "Cancelled" : showDate && date(showDate) <= Date.now() && own ? "Past Showtime" : "Active";
  return `<article class="booking-card">${poster(title,'loading="lazy"')}<div><h3>${esc(title)}</h3>
  <p>${showDate ? esc(fullDate(showDate)+" · "+time(showDate)) : "Showtime unavailable"} · ${esc(hall)}</p>
  <p>Row ${esc(seat.rowNumber ?? "—")} · Seat ${esc(seat.seatNumber ?? "—")} · ${seat.isVip == null ? "Seat type unavailable" : seat.isVip ? "VIP" : "Standard"}</p>
  <span class="badge ${label==="Active"?"good":""}">${label}</span>
  ${own ? `<div class="actions">${b.hasTicket ? button("View Ticket","ownTicket:"+id,"small") : ""}${isUpcoming(b) ? button("Cancel Booking","cancelBooking:"+id,"small danger") : ""}</div>` : ""}</div></article>`;
}
let ownBookings = [];
async function profile(version) {
  const userId = auth.user.id;
  const [user, bookings] = await Promise.all([
    api("User/GetUser/"+path(userId)),api("Booking/GetBookingsByUserId?userId="+path(userId))
  ]);
  const list = await enrichBookings(arr(bookings));
  const withTickets = await Promise.all(list.map(async b => {
    if (b.ticket?.ticketCode || b.ticketCode || b.ticketId) return {...b,hasTicket:true};
    try {
      const ticket = await api("Ticket/GetTicketByBookingId/"+path(bookingId(b)));
      return {...b,hasTicket:!!ticket?.ticketCode};
    } catch { return b; }
  }));
  if (version !== renderId) return;
  ownBookings = withTickets;
  auth.user = {...auth.user,...user,id:userId};
  header();
  main.innerHTML = `<div class="wrap pad">${sectionHead("Your account","My Profile")}
    <div class="profile"><section class="panel"><p class="eyebrow">${esc(auth.role)}</p><h2>Account Information</h2>
    <dl><dt>Username</dt><dd>${esc(auth.user.userName)}</dd><dt>Email</dt><dd>${esc(auth.user.email)}</dd></dl>
    <div class="actions">${button("Edit Profile","editProfile")}${button("Change Password","password")}</div>
    <div id="profile-form"></div><hr>${button("Log Out","logout","danger")}<p id="logout-error" class="error" role="alert"></p></section>
    <section class="panel"><div id="my-bookings"></div></section></div></div>`;
  showBookings();
}
function showBookings() {
  const root = $("#my-bookings");
  if (!root) return;
  const list = ownBookings.filter(b=>isUpcoming(b)===(bookingTab==="active"));
  root.innerHTML = sectionHead("Ticket history","My Bookings",button("Active Bookings","bookingTab:active",bookingTab==="active"?"primary small":"small") +
    button("Past Bookings","bookingTab:past",bookingTab==="past"?"primary small":"small")) +
    (list.length ? list.map(b=>bookingCard(b,true)).join("") : empty("No "+bookingTab+" bookings."));
}
function accountForm(password = false) {
  const modal = !staff();
  const root = modal ? dialog : $("#profile-form");
  const content = formBody(password ? field("Current Password","currentPassword","","password",'autocomplete="current-password"') +
    field("New Password","newPassword","","password",'autocomplete="new-password"') +
    field("Confirm New Password","confirm","","password",'autocomplete="new-password"') :
    field("Username","userName",auth.user.userName) + field("Email","email",auth.user.email,"email"),
    password ? "Change Password" : "Save Changes", modal ? "close" : "cancelProfile");
  if (modal) openDialog(password ? "Change Password" : "Edit Profile", content, "Your account");
  else root.innerHTML = content;
  const form = $("form",root);
  bindForm(form, async data => {
    if (password) {
      if (data.newPassword !== data.confirm) throw new Error("The new passwords must match.");
      await api("Auth/ChangePassword","POST",{currentPassword:data.currentPassword,newPassword:data.newPassword});
      if (root.contains(form)) {
        if (modal) dialog.close();
        else root.innerHTML = "";
      }
    } else {
      await api("User/UpdateUser/"+path(auth.user.id),"PUT",data);
      if (modal && root.contains(form)) dialog.close();
      if (currentPage === "profile") await render();
    }
    notify(password ? "Password changed." : "Profile updated.");
  });
}
const actions = {
  theme() {
    const theme = document.documentElement.dataset.theme === "dark" ? "light" : "dark";
    document.documentElement.dataset.theme = theme;
    localStorage.setItem("cinemago.theme",theme); header();
  },
  login:()=>loginPanel(), register:()=>loginPanel(true), close:()=>{resume=null;dialog.close();},
  retry:render, heroPrev:()=>refreshHero(-1), heroNext:()=>refreshHero(1),
  datesBack:()=>$("#dates").scrollBy({left:-220,behavior:"smooth"}), datesNext:()=>$("#dates").scrollBy({left:220,behavior:"smooth"}),
  day:id=>{scheduleFilter.day=id;scheduleFilter.hall="";showtimes();},
  hallFilter:id=>{scheduleFilter.hall=id;showtimes();},
  book:id=>requireLogin(()=>bookingStart(id)), seat:id=>requireLogin(async ()=>{
    try { await seatsStart(id); } catch(error) { if (dialog.open) openDialog("Couldn’t load seats",empty(error.message)+button("Try Again","seat:"+id)); }
  }),
  bookingsLink:()=>{dialog.close();bookingTab="active";if(location.hash==="#profile") render();else location.hash="profile";},
  bookingTab:id=>{bookingTab=id;showBookings();},
  ownTicket:ticketByBooking,
  cancelBooking:id=>confirmAction("Cancel this booking?","The selected seat will become available again.","Cancel Booking",async ()=>{
    await api("Booking/CancelBooking/"+path(id),"PUT",{id:Number(id)});
    bookingTab="past";await render();
  }),
  editProfile:()=>accountForm(), password:()=>accountForm(true),
  cancelProfile:()=>{$("#profile-form").innerHTML="";},
  async logout() {
    try { await api("Auth/Logout","POST"); } catch (error) {
      if (error.status !== 401) { $("#logout-error").textContent=error.message; return; }
    }
    auth.clear();bookingTab="active";location.hash="home";await render();
  }
};
document.addEventListener("click", async event => {
  const control = event.target.closest("[data-action]");
  if (!control || control.disabled) return;
  event.preventDefault();
  const [action,...parts] = control.dataset.action.split(":");
  try { await actions[action]?.(parts.join(":"), control); }
  catch (error) {
    if (dialog.open) { const alert = $(".error",dialog); if (alert) alert.textContent=error.message; else notify(error.message); }
    else openDialog("CinemaGO",empty(error.message));
  }
});
document.addEventListener("input",event=>{
  if (event.target.id==="movie-search") { movieSearch=event.target.value;filterMovies(); }
});
document.addEventListener("change",event=>{
  const value=event.target.value;
  if (event.target.id==="movie-genre") { movieGenre=value;filterMovies(); }
  if (event.target.id==="show-time") { scheduleFilter.time=value;showtimes(); }
  if (event.target.id==="show-genre") { scheduleFilter.genre=value;showtimes(); }
});
dialog.addEventListener("cancel",()=>{resume=null;});
window.addEventListener("hashchange",()=>{
  if (location.hash === "#main") return;
  if (dialog.open) dialog.close();
  resume=null;
  window.scrollTo(0,0);
  render();
});
document.addEventListener("DOMContentLoaded",async ()=>{
  document.documentElement.dataset.theme = localStorage.getItem("cinemago.theme") ?? "light";
  footer();header();
  if (auth.token) {
    try { await loadIdentity(auth.token); } catch(error) { notify(error.message); }
  }
  render();
});
