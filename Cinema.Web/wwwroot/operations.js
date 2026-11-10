const panels = [["bookings","All Bookings"],["tickets","Tickets"],["sessions","Manage Sessions"],["halls","Manage Halls & Seats"],["users","Users"]];
let operation = "bookings", operationVersion = 0, selectedHall = null, selectedSession = "", scheduledMovie = null;
let opSessions = [], opHalls = [], opMovies = [], opSeats = [], opUsers = [], opTickets = [];
const opRoot = () => $("#operation");
const opForm = () => $("#operation-form");
const row = (title, description, actions = "") => `<article class="show-row"><div><strong>${esc(title)}</strong><p>${esc(description)}</p></div><div class="actions">${actions}</div></article>`;
const select = (label,name,items,value,first) => `<label>${label}<select name="${name}" required>${options(items,value,first)}</select></label>`;
const table = (headers, rows) => `<div class="table-wrap"><table><thead><tr>${headers.map(h=>`<th scope="col">${h}</th>`).join("")}</tr></thead><tbody>${rows.map(r=>"<tr>"+r.map(c=>"<td>"+c+"</td>").join("")+"</tr>").join("")}</tbody></table></div>`;
function operationsHome(version) {
  main.innerHTML = `<div class="wrap pad"><section class="ops-hero"><div><p class="eyebrow"><span class="live-dot"></span> ${esc(auth.role)} workspace</p><h1>Cinema Operations</h1><p>Manage screenings, bookings and your cinema’s day-to-day operations.</p></div><span class="ops-mark" aria-hidden="true">GO</span></section>
    <div class="operations-tabs scroller" id="operation-tabs" aria-label="Operations"></div><section class="panel ops-panel" id="operation"></section></div>`;
  loadOperation(version);
}
async function loadOperation(pageVersion = renderId) {
  const root = opRoot();
  if (!root) return;
  const version = ++operationVersion;
  $("#operation-tabs").innerHTML = panels.map(([key,label])=>button(label,"op:"+key,key===operation?"primary":"")).join("");
  root.innerHTML = empty("Loading " + (panels.find(p=>p[0]===operation)?.[1] ?? "") + "…");
  try {
    const data = await ({
      bookings:async()=>Promise.all([api("Session/GetSessions"),api(selectedSession ? "Booking/GetBookingsBySessionId?sessionId="+path(selectedSession) : "Booking/GetAllBookings")]).then(async ([s,b])=>[arr(s),await enrichBookings(arr(b))]),
      tickets:()=>api("Ticket/GetAllTickets"),
      sessions:()=>Promise.all([api("Session/GetSessions"),api("Movie/GetAllMovies"),api("Hall/GetAllHalls")]),
      halls:async()=>{
        const halls=arr(await api("Hall/GetAllHalls"));
        const hall=halls.find(h=>same(h.id,selectedHall));
        return [halls,hall ? arr(await api("Seat/GetSeatsByHallId/"+path(hall.id))) : []];
      },
      users:async()=>Promise.all(arr(await api("User/GetAllUsers")).map(async u=>{
        try { return {...u,roles:rolesOf(await api("User/GetUserRoleById/"+path(u.id)))}; }
        catch { return {...u,roles:rolesOf(u.roles),roleUnavailable:!rolesOf(u.roles).length}; }
      }))
    }[operation])();
    if (version!==operationVersion || pageVersion!==renderId) return;
    ({
      bookings:()=>showAllBookings(...data),
      tickets:()=>{opTickets=arr(data);root.innerHTML=sectionHead("Box office","Tickets")+`<label class="short">Search by Ticket Code<input id="ticket-search" type="search" placeholder="Enter ticket code"></label><div id="ticket-results"></div>`;showTickets();},
      sessions:()=>{[opSessions,opMovies,opHalls]=data.map(arr);showSessions();},
      halls:()=>{[opHalls,opSeats]=data;showHalls();},
      users:()=>{opUsers=data;showUsers();}
    }[operation])();
  } catch(error) {
    if (version===operationVersion && pageVersion===renderId) root.innerHTML=empty(error.message)+button("Try Again","op:"+operation);
  }
}
function showAllBookings(allSessions, bookings) {
  opRoot().innerHTML=sectionHead("Box office","All Bookings")+`<label class="short">Filter by session<select id="booking-session">${options(allSessions.map(s=>[s.id,sessionTitle(s)+" · "+sessionLabel(s)]),selectedSession,"All Sessions")}</select></label>`+
    (bookings.length ? table(["Booking","Movie & Showtime","Seat","Status","Actions"],bookings.map(b=>[
      "#"+esc(bookingId(b)),esc(sessionTitle(b.displaySession))+(starts(b.displaySession) ? "<br>"+esc(sessionLabel(b.displaySession)) : ""),
      esc(b.displaySeat.rowNumber??"—")+" / "+esc(b.displaySeat.seatNumber??"—"),
      `<span class="badge ${isActive(b)?"good":""}">${isActive(b)?"Active":"Cancelled"}</span>`,
      button("View","bookingDetail:"+bookingId(b),"small")+button("Delete","deleteBooking:"+bookingId(b),"small danger")
    ])) : empty("No bookings found."));
}
function showTickets(query="") {
  const items=opTickets.filter(t=>String(t.ticketCode??"").toLowerCase().includes(query.trim().toLowerCase()));
  $("#ticket-results").innerHTML=items.length ? table(["Ticket Code","Final Price","Booking","Actions"],items.map(t=>[
    esc(t.ticketCode),money(t.finalPrice),"#"+esc(t.bookingId),button("View","staffTicket:"+t.id,"small")
  ])) : empty("No tickets match your search.");
}
async function bookingDetail(id) {
  const b=await api("Booking/GetBookingById?id="+path(id));
  const [detail]=await enrichBookings([b]);
  if (auth.role !== "SuperAdmin") {
    openDialog("Booking #"+id,bookingCard(detail)+`<p>User ID: ${esc(b.userId ?? "Unavailable")}</p>`,"Box office");
    return;
  }
  const session = detail.displaySession;
  const seat = detail.displaySeat;
  const title = sessionTitle(session);
  const active = isActive(b);
  const start = starts(session);
  const hall = session.hallName ?? (session.hallId ? "Hall " + session.hallId : "Unavailable");
  const facts = [
    ["Showtime", start ? fullDate(start) + " · " + time(start) : "Unavailable"],
    ["Hall", hall],
    ["Row & Seat", "Row " + (seat.rowNumber ?? "—") + " · Seat " + (seat.seatNumber ?? "—")],
    ["Seat Type", seat.isVip == null ? "Unavailable" : seat.isVip ? "VIP" : "Standard"]
  ];
  openDialog("Booking #"+id, `<article class="booking-detail">
    <div class="booking-detail-summary">${poster(title)}<div><p class="eyebrow">Screening</p>
      <h3>${esc(title)}</h3><span class="badge ${active ? "good" : ""}">${active ? "Active" : "Cancelled"}</span></div></div>
    <dl class="booking-detail-facts">${facts.map(([label,value]) => `<div><dt>${label}</dt><dd>${esc(value)}</dd></div>`).join("")}
      <div class="booking-detail-user"><dt>User ID</dt><dd>${esc(b.userId ?? "Unavailable")}</dd></div></dl>
  </article><div class="actions form-actions">${button("Close","close")}</div>`,"Box office");
}
function showSessions() {
  opRoot().innerHTML=sectionHead("Screenings","Manage Sessions",button("+ Add Session","addSession","primary"))+'<div id="operation-form"></div>'+
    (opSessions.length ? table(["Movie","Hall","Date","Start","Standard","VIP","Actions"],opSessions.map(s=>[
      esc(sessionTitle(s)),esc(s.hallName??"Hall "+s.hallId),esc(fullDate(starts(s))),esc(time(starts(s))),money(s.standardPrice),money(s.vipPrice),
      button("Edit","editSession:"+s.id,"small")+button("Delete","deleteSession:"+s.id,"small danger")
    ])) : empty("No sessions have been scheduled."));
  if (scheduledMovie!==null) { sessionForm(null,scheduledMovie);scheduledMovie=null; }
}
function inlineForm(root,title,fields,label,task) {
  root.innerHTML=`<section class="inline-form"><h3>${esc(title)}</h3>${formBody(fields,label,"clearInline")}</section>`;
  bindForm($("form",root),task);
  root.scrollIntoView({block:"nearest"});
}
function sessionForm(s, movieId) {
  const value=starts(s ?? {});
  inlineForm(opForm(),s?"Edit "+sessionTitle(s):"Add Session",
    select("Movie","movieId",opMovies.map(m=>[m.id,m.title]),s?.movieId??movieId,"Choose a movie")+
    select("Hall","hallId",opHalls.map(h=>[h.id,h.hallName]),s?.hallId,"Choose a hall")+
    field("Date","date",value?dayKey(value):"","date")+field("Start Time","time",value?time(value):"","time")+
    field("Standard Price","standardPrice",s?.standardPrice,"number",'min="0" step="0.01"')+
    field("VIP Price","vipPrice",s?.vipPrice,"number",'min="0" step="0.01"'),
    s?"Save Changes":"Add Session",async data=>{
      const localStart=data.date+"T"+data.time+":00";
      await api(s ? "Session/UpdateSession/"+path(s.id) : "Session/AddSession",s?"PUT":"POST",{
        startsAt:localStart,standardPrice:Number(data.standardPrice),vipPrice:Number(data.vipPrice),movieId:Number(data.movieId),hallId:Number(data.hallId)
      });
      await loadOperation();notify("Session saved.");
    });
}
function showHalls() {
  const hall=opHalls.find(h=>same(h.id,selectedHall));
  opRoot().innerHTML=sectionHead("Venues","Manage Halls & Seats",button("+ Add Hall","addHall","primary"))+'<div id="operation-form"></div>'+
    (opHalls.length ? opHalls.map(h=>row(h.hallName,hall===h?"Seats shown below":"Cinema hall",button("View Seats","selectHall:"+h.id,hall===h?"primary":"")+
      button("Edit","editHall:"+h.id,"small")+button("Delete","deleteHall:"+h.id,"small danger"))).join("") : empty("No halls found."))+
    (hall ? '<div class="subpanel">'+sectionHead("Seating",esc(hall.hallName),button("+ Add Seat","addSeat","primary"))+'<div id="seat-form"></div>'+
      (opSeats.length ? table(["Row","Seat","Type","Actions"],opSeats.map(s=>[esc(s.rowNumber),esc(s.seatNumber),s.isVip?"VIP":"Standard",
        button("Edit","editSeat:"+s.id,"small")+button("Delete","deleteSeat:"+s.id,"small danger")])) : empty("No seats in this hall."))+"</div>" : "");
}
function hallForm(h) {
  inlineForm(opForm(),h?"Edit "+h.hallName:"Add Hall",field("Hall Name","hallName",h?.hallName),h?"Save Changes":"Add Hall",async data=>{
    await api(h?"Hall/UpdateHall/"+path(h.id):"Hall/AddHall",h?"PUT":"POST",data);
    await loadOperation();notify("Hall saved.");
  });
}
function seatForm(s) {
  inlineForm($("#seat-form"),s?`Edit Row ${s.rowNumber}, Seat ${s.seatNumber}`:"Add Seat",
    (s?"":field("Row Number","rowNumber","","number",'min="1" step="1"')+field("Seat Number","seatNumber","","number",'min="1" step="1"'))+
    select("Seat Type","isVip",[["false","Standard"],["true","VIP"]],String(!!s?.isVip)),
    s?"Save Changes":"Add Seat",async data=>{
      const isVip=data.isVip==="true";
      await api(s?"Seat/UpdateSeat/"+path(s.id):"Seat/AddSeat",s?"PUT":"POST",s?{isVip}:{isVip,rowNumber:Number(data.rowNumber),seatNumber:Number(data.seatNumber),hallId:Number(selectedHall)});
      await loadOperation();notify("Seat saved.");
    });
}
function userActions(u) {
  const view=button("View","viewUser:"+u.id,"small");
  if (u.roleUnavailable || u.roles.includes("SuperAdmin")) return view;
  const admin=u.roles.includes("Admin");
  const customer=u.roles.includes("Customer");
  const superAdmin=auth.role==="SuperAdmin";
  if (!(superAdmin && admin) && !(customer && !admin)) return view;
  return view+button("Edit","editUser:"+u.id,"small")+button("Delete","deleteUser:"+u.id,"small danger")+
    (superAdmin ? button(admin?"Remove Admin Role":"Make Admin",(admin?"removeAdmin:":"makeAdmin:")+u.id,"small") : "");
}
function showUsers() {
  opRoot().innerHTML=sectionHead("Accounts","Users")+'<div id="operation-form"></div>'+
    (opUsers.length ? table(["Username","Email","Roles","Actions"],opUsers.map(u=>[
      esc(u.userName),esc(u.email),u.roles.length ? u.roles.map(role=>`<span class="badge">${esc(role)}</span>`).join(" ") : "Roles unavailable",userActions(u)
    ])) : empty("No users found."));
}
function editUser(u) {
  inlineForm(opForm(),"Edit "+u.userName,field("Username","userName",u.userName)+field("Email","email",u.email,"email"),"Save Changes",async data=>{
    await api("User/UpdateUser/"+path(u.id),"PUT",data);await loadOperation();notify("User updated.");
  });
}
function movieForm(m) {
  const root=$("#movie-form");
  inlineForm(root,m?"Edit "+m.title:"Add Movie",
    field("Title","title",m?.title)+`<label>Description<textarea name="description" required>${esc(m?.description)}</textarea></label>`+
    field("Duration in Minutes","durationMinutes",m?.durationMinutes,"number",'min="1" step="1"')+
    select("Age Rating","ageRating",config.AGE_RATINGS.map((a,i)=>[i,a]),m?.ageRating,"Choose a rating")+
    `<fieldset class="genre-picker"><legend>Genres</legend><div id="genre-choices">${genreChoices(genreIds(m??{}))}</div></fieldset>`,m?"Save Changes":"Add Movie",async (data,form)=>{
      const ids=form.getAll("genreIds").map(Number);
      if (!ids.length) throw new Error("Select at least one genre.");
      await api(m?"Movie/UpdateMovie/"+path(m.id):"Movie/AddMovie",m?"PUT":"POST",{
        title:data.title,description:data.description,durationMinutes:Number(data.durationMinutes),ageRating:Number(data.ageRating),genreIds:ids
      });
      await render();notify("Movie saved.");
    });
}
function genreChoices(ids) {
  return genres.length ? genres.map(g=>`<label class="chip"><input name="genreIds" type="checkbox" value="${esc(g.id)}" ${ids.some(id=>same(id,g.id))?"checked":""}> ${esc(g.genreName)}</label>`).join("") : '<p>Add a genre using Manage Genres.</p>';
}
async function refreshGenreDialog() {
  await catalog();
  if (movieGenre && !genres.some(g=>same(g.id,movieGenre))) movieGenre="";
  $("#movie-filters").innerHTML=filters();
  filterMovies();
  const choices=$("#genre-choices");
  if (choices) choices.innerHTML=genreChoices([...choices.querySelectorAll(":checked")].map(i=>i.value));
  genreDialog();
}
function genreDialog(edit, remove) {
  openDialog("Manage Genres", formBody(field("Genre Name","genreName"),"Add Genre","")+
    (genres.length ? genres.map(g=>row(g.genreName,"",button("Edit","editGenre:"+g.id,"small")+button("Delete","deleteGenre:"+g.id,"small danger"))).join("") : empty("No genres found."))+
    '<div id="genre-action"></div><div class="actions form-actions">'+button("Close","close")+"</div>","Catalog");
  bindForm($("form",dialog),async data=>{await api("Genre/AddGenre","POST",data);await refreshGenreDialog();});
  if (!edit && !remove) return;
  const g=edit??remove;
  const root=$("#genre-action",dialog);
  inlineForm(root,edit?"Rename "+g.genreName:"Delete "+g.genreName+"?",edit?field("Genre Name","genreName",g.genreName):"",
    edit?"Save Changes":"Delete Genre",async data=>{
      await api((edit?"Genre/UpdateGenre/":"Genre/DeleteGenre/")+path(g.id),edit?"PUT":"DELETE",edit?data:undefined);
      await refreshGenreDialog();
    });
}
const find = (items,id)=>items.find(i=>same(i.id,id));
const deleteOperation = (title,message,route,label) => confirmAction(title,message,label,async()=>{
  await api(route,"DELETE");await loadOperation();
});
Object.assign(actions,{
  op:key=>{operation=key;return loadOperation();},
  clearInline:(_,control)=>control.closest(".inline-form")?.remove(),
  bookingDetail,
  deleteBooking:id=>deleteOperation("Delete Booking #"+id+"?","This permanently deletes the selected booking.","Booking/DeleteBooking/"+path(id),"Delete Booking"),
  async staffTicket(id) {
    const t=await api("Ticket/GetTicketById/"+path(id));
    openDialog("Ticket Details",ticketView(t)+`<div class="actions form-actions">${button("View Booking #"+t.bookingId,"bookingDetail:"+t.bookingId)}</div>`,"Box office");
  },
  schedule:id=>{
    scheduledMovie=id;operation="sessions";
    if(location.hash==="#home") return loadOperation();
    location.hash="home";
  },
  addSession:()=>sessionForm(),editSession:id=>sessionForm(find(opSessions,id)),
  deleteSession:id=>{
    const s=find(opSessions,id);
    deleteOperation("Delete "+sessionTitle(s)+"?",sessionLabel(s),"Session/DeleteSession/"+path(id),"Delete Session");
  },
  selectHall:id=>{selectedHall=id;return loadOperation();},
  addHall:()=>hallForm(),editHall:id=>hallForm(find(opHalls,id)),
  deleteHall:id=>deleteOperation("Delete "+find(opHalls,id).hallName+"?","Confirm deletion of this hall.","Hall/DeleteHall/"+path(id),"Delete Hall"),
  addSeat:()=>seatForm(),editSeat:id=>seatForm(find(opSeats,id)),
  deleteSeat:id=>{
    const s=find(opSeats,id);
    deleteOperation("Delete Seat "+s.seatNumber+"?","Row "+s.rowNumber,"Seat/DeleteSeat/"+path(id),"Delete Seat");
  },
  async viewUser(id) {
    const [u,b]=await Promise.all([api("User/GetUser/"+path(id)),api("Booking/GetBookingsByUserId?userId="+path(id))]);
    const list=await enrichBookings(arr(b));
    openDialog(u.userName,`<p>${esc(u.email)}</p><h3>Booking History</h3>`+(list.length?list.map(b=>bookingCard(b)).join(""):empty("No bookings found.")),"User profile");
  },
  editUser:id=>editUser(find(opUsers,id)),
  deleteUser:id=>deleteOperation("Delete "+find(opUsers,id).userName+"?","Confirm deletion of this account.","User/DeleteUser/"+path(id),"Delete User"),
  makeAdmin:id=>confirmAction("Make "+find(opUsers,id).userName+" an Admin?","This grants access to cinema management.","Make Admin",async()=>{
    await api("User/MakeAdmin/"+path(id),"POST");await loadOperation();
  }),
  removeAdmin:id=>deleteOperation("Remove the Admin role from "+find(opUsers,id).userName+"?","This changes the account’s management permissions.","User/RemoveAdminRole/"+path(id),"Remove Admin Role"),
  addMovie:()=>movieForm(),editMovie:id=>movieForm(movie(id)),
  deleteMovie:id=>confirmAction("Delete "+movie(id).title+"?","Confirm deletion of this movie.","Delete Movie",async()=>{
    await api("Movie/DeleteMovie/"+path(id),"DELETE");await render();
  }),
  genres:()=>genreDialog(),editGenre:id=>genreDialog(find(genres,id)),deleteGenre:id=>genreDialog(null,find(genres,id))
});
document.addEventListener("change",event=>{
  if(event.target.id==="booking-session") {selectedSession=event.target.value;loadOperation();}
});
document.addEventListener("input",event=>{
  if(event.target.id==="ticket-search") showTickets(event.target.value);
});
