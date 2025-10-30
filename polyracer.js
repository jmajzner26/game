// PolyRacer - Top-down racing with multiple tracks and local leaderboard
class PolyRacer {
	constructor() {
		this.canvas = document.getElementById('polyracer-canvas');
		this.ctx = this.canvas.getContext('2d');
		this.width = this.canvas.width;
		this.height = this.canvas.height;
		this.trackId = 'oval';
		this.tracks = this.buildTracks();
		this.bind();
		this.reset();
		this.draw();
		this.refreshLeaderboard();
	}

	buildTracks() {
		// Each track: { name, route: array of polygon points for walls, startLine: [p1,p2], laps:1 }
		const margin = 40;
		const makeOval = () => ({
			name: 'oval',
			walls: [
				// outer rectangle
				[{x:margin,y:margin},{x:this.width-margin,y:margin},{x:this.width-margin,y:this.height-margin},{x:margin,y:this.height-margin}],
				// inner rectangle (track width)
				[{x:margin+120,y:margin+80},{x:this.width-margin-120,y:margin+80},{x:this.width-margin-120,y:this.height-margin-80},{x:margin+120,y:this.height-margin-80}],
			],
			startLine: [{x:this.width/2-60,y:this.height-margin-80},{x:this.width/2+60,y:this.height-margin-80}],
			laps: 1
		});
		const makeChicane = () => ({
			name: 'chicane',
			walls: [
				[{x:margin,y:margin},{x:this.width-margin,y:margin},{x:this.width-margin,y:this.height-margin},{x:margin,y:this.height-margin}],
				[{x:margin+90,y:margin+90},{x:this.width-margin-90,y:margin+90},{x:this.width-margin-180,y:this.height/2},{x:this.width-margin-90,y:this.height-margin-90},{x:margin+90,y:this.height-margin-90},{x:margin+180,y:this.height/2}],
			],
			startLine: [{x:this.width/2-60,y:this.height/2+100},{x:this.width/2+60,y:this.height/2+100}],
			laps: 1
		});
		const makeSprint = () => ({
			name: 'sprint',
			walls: [
				[{x:margin,y:margin},{x:this.width-margin,y:margin},{x:this.width-margin,y:this.height-margin},{x:margin,y:this.height-margin}],
				[{x:margin+200,y:margin+60},{x:this.width-margin-60,y:margin+120},{x:this.width-margin-200,y:this.height-margin-60},{x:margin+60,y:this.height-margin-120}],
			],
			startLine: [{x:margin+120,y:this.height/2-40},{x:margin+120,y:this.height/2+40}],
			laps: 1
		});
		const makePoly = () => ({
			name: 'poly',
			walls: [
				// outer irregular polygon
				[
					{x:20,y:60},{x:this.width-40,y:40},{x:this.width-30,y:this.height/2-80},
					{x:this.width-60,y:this.height-40},{x:80,y:this.height-30},{x:40,y:this.height/2+60}
				],
				// inner offset polygon for road width
				[
					{x:120,y:120},{x:this.width-140,y:110},{x:this.width-130,y:this.height/2-30},
					{x:this.width-160,y:this.height-110},{x:150,y:this.height-120},{x:110,y:this.height/2+20}
				],
			],
			startLine: [{x:this.width/2-50,y:this.height-120},{x:this.width/2+50,y:this.height-120}],
			laps: 1
		});
		return { oval: makeOval(), chicane: makeChicane(), sprint: makeSprint(), poly: makePoly() };
	}

	bind() {
		document.getElementById('track-select').addEventListener('change', (e) => {
			this.trackId = e.target.value;
			document.getElementById('pr-track').textContent = this.trackId;
			this.reset();
			this.refreshLeaderboard();
		});
		document.getElementById('pr-start-btn').addEventListener('click', () => this.start());
		document.getElementById('pr-restart-btn').addEventListener('click', () => this.reset());
		document.addEventListener('keydown', (e) => {
			const active = !document.getElementById('polyracer-game').classList.contains('hidden');
			if (!active) return;
			if (!this.running && e.code === 'Space') { this.start(); return; }
			if (e.key === 'r' || e.key === 'R') { this.reset(); return; }
			if (!this.running) return;
			if (e.key === 'ArrowUp') this.controls.accel = true;
			if (e.key === 'ArrowDown') this.controls.brake = true;
			if (e.key === 'ArrowLeft') this.controls.left = true;
			if (e.key === 'ArrowRight') this.controls.right = true;
		});
		document.addEventListener('keyup', (e) => {
			const active = !document.getElementById('polyracer-game').classList.contains('hidden');
			if (!active) return;
			if (e.key === 'ArrowUp') this.controls.accel = false;
			if (e.key === 'ArrowDown') this.controls.brake = false;
			if (e.key === 'ArrowLeft') this.controls.left = false;
			if (e.key === 'ArrowRight') this.controls.right = false;
		});
	}

	reset() {
		this.running = false;
		this.controls = { accel: false, brake: false, left: false, right: false };
		const t = this.tracks[this.trackId];
		// start near start line, pointing up
		this.car = { x: (t.startLine[0].x + t.startLine[1].x)/2, y: t.startLine[0].y + 30, angle: -Math.PI/2, v: 0 };
		this.timeStart = 0;
		this.currentTime = 0;
		this.best = this.getBestTime();
		document.getElementById('pr-best').textContent = this.best ? this.formatMs(this.best) : '-';
		document.getElementById('pr-time').textContent = '0.000';
		this.showOverlay('PolyRacer', 'Arrow keys to drive • Space to start');
		this.draw();
	}

	start() {
		if (this.running) return;
		this.hideOverlay();
		this.running = true;
		this.timeStart = performance.now();
		if (typeof window !== 'undefined' && typeof window.va === 'function') window.va('polyracer_start', { track: this.trackId });
		requestAnimationFrame((t) => this.loop(t));
	}

	loop(t) {
		if (!this.running) return;
		this.update();
		this.draw();
		this.currentTime = performance.now() - this.timeStart;
		document.getElementById('pr-time').textContent = this.formatMs(this.currentTime);
		requestAnimationFrame((tt) => this.loop(tt));
	}

	update() {
		// simple physics
		const accel = this.controls.accel ? 0.12 : 0;
		const brake = this.controls.brake ? 0.2 : 0;
		const drag = 0.02;
		this.car.v += accel - brake - drag * this.car.v;
		this.car.v = Math.max(Math.min(this.car.v, 6), -2);
		if (this.controls.left) this.car.angle -= 0.045 * (1 + 0.2 * Math.abs(this.car.v));
		if (this.controls.right) this.car.angle += 0.045 * (1 + 0.2 * Math.abs(this.car.v));
		this.car.x += Math.cos(this.car.angle) * this.car.v;
		this.car.y += Math.sin(this.car.angle) * this.car.v;

		// collisions with walls -> stop and reset lap start time (penalty)
		if (this.collidesWalls()) {
			this.car.v *= -0.3;
		}

		// check crossing start line going upward (angle near -PI/2) to finish lap
		const t = this.tracks[this.trackId];
		if (this.crossedStartLine(t.startLine) && performance.now() - this.timeStart > 1500) {
			this.finishLap();
		}
	}

	crossedStartLine([p1, p2]) {
		// line equation, detect if car moved from one side to another between frames
		if (!this.prevPos) { this.prevPos = { x: this.car.x, y: this.car.y }; return false; }
		const side = (pt) => (p2.x - p1.x)*(pt.y - p1.y) - (p2.y - p1.y)*(pt.x - p1.x);
		const s1 = side(this.prevPos);
		const s2 = side(this.car);
		this.prevPos = { x: this.car.x, y: this.car.y };
		return s1 < 0 && s2 >= 0; // crossed in forward direction
	}

	finishLap() {
		this.running = false;
		const ms = Math.round(this.currentTime);
		this.saveTime(ms);
		this.best = this.getBestTime();
		document.getElementById('pr-best').textContent = this.best ? this.formatMs(this.best) : '-';
		this.refreshLeaderboard();
		this.showOverlay('Finish!', `Lap: ${this.formatMs(ms)}`);
		if (typeof window !== 'undefined' && typeof window.va === 'function') window.va('polyracer_finish', { track: this.trackId, ms });
	}

	formatMs(ms) {
		return (ms/1000).toFixed(3);
	}

	collidesWalls() {
		const carPoly = this.carPolygon();
		const t = this.tracks[this.trackId];
		// collide if outside outer wall OR inside inner wall
		const outer = t.walls[0];
		const inner = t.walls[1];
		return !this.polyInside(carPoly, outer) || this.polyInside(carPoly, inner);
	}

	carPolygon() {
		// rectangle around car
		const w = 18, h = 10;
		const cx = this.car.x, cy = this.car.y, a = this.car.angle;
		const pts = [
			{ x: -h, y: -w/2 },
			{ x: -h, y:  w/2 },
			{ x:  h, y:  w/2 },
			{ x:  h, y: -w/2 },
		];
		return pts.map(p => ({ x: cx + p.x*Math.cos(a) - p.y*Math.sin(a), y: cy + p.x*Math.sin(a) + p.y*Math.cos(a) }));
	}

	polyInside(poly, boundary) {
		// return true if all vertices inside polygon boundary (ray casting)
		return poly.every(pt => this.pointInPolygon(pt, boundary));
	}

	pointInPolygon(pt, vs) {
		let inside = false;
		for (let i=0, j=vs.length-1; i<vs.length; j=i++) {
			const xi = vs[i].x, yi = vs[i].y;
			const xj = vs[j].x, yj = vs[j].y;
			const intersect = ((yi>pt.y)!=(yj>pt.y)) && (pt.x < (xj-xi)*(pt.y-yi)/(yj-yi)+xi);
			if (intersect) inside = !inside;
		}
		return inside;
	}

	draw() {
		const ctx = this.ctx; ctx.fillStyle = '#111'; ctx.fillRect(0,0,this.width,this.height);
		// draw track
		const t = this.tracks[this.trackId];
		ctx.fillStyle = '#2c2c2c';
		ctx.beginPath(); this.pathFromPoly(t.walls[0]); ctx.fill();
		ctx.fillStyle = '#0a0a0a';
		ctx.beginPath(); this.pathFromPoly(t.walls[1]); ctx.fill();
		// start line
		ctx.strokeStyle = '#ffffff'; ctx.lineWidth = 3; ctx.beginPath(); ctx.moveTo(t.startLine[0].x, t.startLine[0].y); ctx.lineTo(t.startLine[1].x, t.startLine[1].y); ctx.stroke();
		// car
		ctx.save(); ctx.translate(this.car.x, this.car.y); ctx.rotate(this.car.angle); ctx.fillStyle = '#4ecdc4'; ctx.fillRect(-10, -6, 20, 12); ctx.restore();
	}

	pathFromPoly(poly) {
		const ctx = this.ctx; ctx.moveTo(poly[0].x, poly[0].y); for (let i=1;i<poly.length;i++) ctx.lineTo(poly[i].x, poly[i].y); ctx.closePath();
	}

	showOverlay(title, msg) {
		const o = document.getElementById('polyracer-overlay');
		document.getElementById('pr-overlay-title').textContent = title;
		document.getElementById('pr-overlay-message').textContent = msg;
		o.classList.remove('hidden');
	}
	
	hideOverlay() {
		document.getElementById('polyracer-overlay').classList.add('hidden');
	}

	leaderboardKey() { return `polyracer_lb_${this.trackId}`; }

	getLeaderboard() {
		try { return JSON.parse(localStorage.getItem(this.leaderboardKey())||'[]'); } catch { return []; }
	}

	saveTime(ms) {
		const name = prompt('New record! Enter your name:', 'Player');
		const entry = { name: (name||'Player').slice(0,16), ms };
		let lb = this.getLeaderboard();
		lb.push(entry);
		lb.sort((a,b)=>a.ms-b.ms);
		lb = lb.slice(0,100);
		localStorage.setItem(this.leaderboardKey(), JSON.stringify(lb));
	}

	getBestTime() {
		const lb = this.getLeaderboard();
		return lb.length ? lb[0].ms : null;
	}

	refreshLeaderboard() {
		const list = document.getElementById('pr-leaderboard');
		list.innerHTML = '';
		const lb = this.getLeaderboard();
		lb.forEach((e, idx) => {
			const li = document.createElement('li');
			li.textContent = `${idx+1}. ${e.name} — ${this.formatMs(e.ms)}s`;
			list.appendChild(li);
		});
	}
}

let polyRacer;
window.addEventListener('load', () => {
	if (document.getElementById('polyracer-canvas')) {
		polyRacer = new PolyRacer();
	}
});
