// 3D Racer - High-performance 3D racing game with low-poly visuals
class Racer3D {
	constructor() {
		this.container = document.getElementById('racer3d-container');
		if (!this.container) return;
		
		this.trackId = 'city';
		this.scene = null;
		this.camera = null;
		this.renderer = null;
		this.world = null;
		this.car = null;
		this.carBody = null;
		this.track = null;
		this.checkpoints = [];
		this.currentCheckpoint = 0;
		this.lapCount = 0;
		this.maxLaps = 3;
		this.raceStart = 0;
		this.lapStart = 0;
		this.currentLapTime = 0;
		this.bestTime = null;
		this.running = false;
		this.controls = { forward: false, backward: false, left: false, right: false };
		this.speed = 0;
		this.position = { x: 0, y: 0, z: 0 };
		
		this.init();
		this.bind();
		this.reset();
		this.animate();
	}

	init() {
		// Scene
		this.scene = new THREE.Scene();
		this.scene.background = new THREE.Color(0x87CEEB);
		this.scene.fog = new THREE.Fog(0x87CEEB, 50, 200);

		// Camera (follow car)
		this.camera = new THREE.PerspectiveCamera(75, 800/600, 0.1, 1000);
		this.camera.position.set(0, 8, 15);

		// Renderer
		this.renderer = new THREE.WebGLRenderer({ antialias: true });
		this.renderer.setSize(800, 600);
		this.renderer.shadowMap.enabled = true;
		this.container.appendChild(this.renderer.domElement);

		// Physics world (simplified - using manual physics for performance)
		// Lighting
		const ambientLight = new THREE.AmbientLight(0xffffff, 0.6);
		this.scene.add(ambientLight);
		
		const dirLight = new THREE.DirectionalLight(0xffffff, 0.8);
		dirLight.position.set(20, 30, 20);
		dirLight.castShadow = true;
		dirLight.shadow.camera.left = -50;
		dirLight.shadow.camera.right = 50;
		dirLight.shadow.camera.top = 50;
		dirLight.shadow.camera.bottom = -50;
		this.scene.add(dirLight);
	}

	bind() {
		document.getElementById('r3d-track-select').addEventListener('change', (e) => {
			this.trackId = e.target.value;
			document.getElementById('r3d-track-name').textContent = this.trackId;
			this.reset();
			this.refreshLeaderboard();
		});
		
		document.getElementById('r3d-start-btn').addEventListener('click', () => this.start());
		
		document.addEventListener('keydown', (e) => {
			const active = !document.getElementById('racer3d-game').classList.contains('hidden');
			if (!active) return;
			
			if (!this.running && (e.code === 'Space' || e.key === ' ')) {
				e.preventDefault();
				this.start();
				return;
			}
			
			if (e.key === 'r' || e.key === 'R') {
				this.reset();
				return;
			}
			
			if (!this.running) return;
			
			switch(e.key.toLowerCase()) {
				case 'w':
				case 'arrowup':
					this.controls.forward = true;
					break;
				case 's':
				case 'arrowdown':
					this.controls.backward = true;
					break;
				case 'a':
				case 'arrowleft':
					this.controls.left = true;
					break;
				case 'd':
				case 'arrowright':
					this.controls.right = true;
					break;
			}
		});
		
		document.addEventListener('keyup', (e) => {
			const active = !document.getElementById('racer3d-game').classList.contains('hidden');
			if (!active) return;
			
			switch(e.key.toLowerCase()) {
				case 'w':
				case 'arrowup':
					this.controls.forward = false;
					break;
				case 's':
				case 'arrowdown':
					this.controls.backward = false;
					break;
				case 'a':
				case 'arrowleft':
					this.controls.left = false;
					break;
				case 'd':
				case 'arrowright':
					this.controls.right = false;
					break;
			}
		});
	}

	buildTrack(trackId) {
		// Clear existing track
		if (this.track) {
			this.scene.remove(this.track);
		}
		this.checkpoints = [];

		// Track configurations with different environments
		const tracks = {
			city: {
				groundColor: 0x444444,
				wallColor: 0x888888,
				ambient: 0x999999,
				points: [
					{x:0,z:0}, {x:50,z:0}, {x:50,z:50}, {x:0,z:50},
					{x:-30,z:50}, {x:-30,z:100}, {x:30,z:100}, {x:30,z:50}
				]
			},
			desert: {
				groundColor: 0xd4a574,
				wallColor: 0xb8935f,
				ambient: 0xffd700,
				points: [
					{x:0,z:0}, {x:60,z:0}, {x:60,z:60}, {x:0,z:60},
					{x:-40,z:60}, {x:-40,z:120}, {x:40,z:120}, {x:40,z:60}
				]
			},
			snow: {
				groundColor: 0xeeeeee,
				wallColor: 0xcccccc,
				ambient: 0xaaccff,
				points: [
					{x:0,z:0}, {x:55,z:0}, {x:55,z:55}, {x:0,z:55},
					{x:-35,z:55}, {x:-35,z:110}, {x:35,z:110}, {x:35,z:55}
				]
			},
			neon: {
				groundColor: 0x1a1a2e,
				wallColor: 0x16213e,
				ambient: 0x0f3460,
				points: [
					{x:0,z:0}, {x:65,z:0}, {x:65,z:65}, {x:0,z:65},
					{x:-45,z:65}, {x:-45,z:130}, {x:45,z:130}, {x:45,z:65}
				]
			}
		};

		const config = tracks[trackId] || tracks.city;
		this.scene.background = new THREE.Color(config.ambient);
		this.scene.fog = new THREE.Fog(config.ambient, 50, 200);

		// Ground
		const groundGeometry = new THREE.PlaneGeometry(300, 300);
		const groundMaterial = new THREE.MeshLambertMaterial({ color: config.groundColor });
		const ground = new THREE.Mesh(groundGeometry, groundMaterial);
		ground.rotation.x = -Math.PI / 2;
		ground.receiveShadow = true;
		this.scene.add(ground);

		// Track surface
		const trackGroup = new THREE.Group();
		
		// Create track path from points
		for (let i = 0; i < config.points.length; i++) {
			const p1 = config.points[i];
			const p2 = config.points[(i + 1) % config.points.length];
			
			// Track segment
			const length = Math.sqrt((p2.x-p1.x)**2 + (p2.z-p1.z)**2);
			const angle = Math.atan2(p2.z-p1.z, p2.x-p1.x);
			const segment = new THREE.Mesh(
				new THREE.PlaneGeometry(length, 20),
				new THREE.MeshLambertMaterial({ color: 0x333333 })
			);
			segment.rotation.x = -Math.PI / 2;
			segment.position.set((p1.x+p2.x)/2, 0.1, (p1.z+p2.z)/2);
			segment.rotation.y = angle;
			trackGroup.add(segment);
			
			// Walls
			const wall1 = new THREE.Mesh(
				new THREE.BoxGeometry(length, 5, 1),
				new THREE.MeshLambertMaterial({ color: config.wallColor })
			);
			const perp = Math.atan2(p2.x-p1.x, -(p2.z-p1.z));
			wall1.position.set(p1.x + Math.cos(perp)*10, 2.5, p1.z + Math.sin(perp)*10);
			wall1.rotation.y = angle;
			trackGroup.add(wall1);
			
			const wall2 = wall1.clone();
			wall2.position.set(p1.x - Math.cos(perp)*10, 2.5, p1.z - Math.sin(perp)*10);
			trackGroup.add(wall2);
			
			// Checkpoint
			this.checkpoints.push({ x: p2.x, z: p2.z });
		}
		
		this.track = trackGroup;
		this.scene.add(trackGroup);

		// Neon lights for neon track
		if (trackId === 'neon') {
			for (let i = 0; i < config.points.length; i++) {
				const light = new THREE.PointLight(0x00ffff, 1, 30);
				light.position.set(config.points[i].x, 5, config.points[i].z);
				this.scene.add(light);
			}
		}
	}

	createCar() {
		if (this.car) {
			this.scene.remove(this.car);
		}

		// Low-poly car
		const carGroup = new THREE.Group();
		
		// Body
		const bodyGeometry = new THREE.BoxGeometry(4, 1.5, 2);
		const bodyMaterial = new THREE.MeshPhongMaterial({ color: 0x4ecdc4, shininess: 100 });
		const body = new THREE.Mesh(bodyGeometry, bodyMaterial);
		body.position.set(0, 1, 0);
		body.castShadow = true;
		carGroup.add(body);
		
		// Wheels
		const wheelGeometry = new THREE.CylinderGeometry(0.5, 0.5, 0.3, 8);
		const wheelMaterial = new THREE.MeshPhongMaterial({ color: 0x111111 });
		const positions = [[-1.5, 0.5, 1.2], [1.5, 0.5, 1.2], [-1.5, 0.5, -1.2], [1.5, 0.5, -1.2]];
		positions.forEach(([x, y, z]) => {
			const wheel = new THREE.Mesh(wheelGeometry, wheelMaterial);
			wheel.rotation.z = Math.PI / 2;
			wheel.position.set(x, y, z);
			wheel.castShadow = true;
			carGroup.add(wheel);
		});
		
		this.car = carGroup;
		this.car.position.set(0, 1, 0);
		this.car.rotation.y = Math.PI / 2;
		this.scene.add(carGroup);
		this.carBody = body;
	}

	reset() {
		this.running = false;
		this.lapCount = 0;
		this.currentCheckpoint = 0;
		this.speed = 0;
		this.controls = { forward: false, backward: false, left: false, right: false };
		
		this.buildTrack(this.trackId);
		this.createCar();
		
		this.car.position.set(0, 1, 0);
		this.car.rotation.y = Math.PI / 2;
		
		this.bestTime = this.getBestTime();
		document.getElementById('r3d-best-time').textContent = this.bestTime ? this.formatTime(this.bestTime) : '-';
		this.refreshLeaderboard();
		
		this.hideHUD();
		this.showOverlay('3D Racer', 'Select track and press Start');
	}

	start() {
		this.running = true;
		this.lapCount = 0;
		this.currentCheckpoint = 0;
		this.raceStart = performance.now();
		this.lapStart = this.raceStart;
		this.currentLapTime = 0;
		
		this.hideOverlay();
		this.showHUD();
		
		if (typeof window !== 'undefined' && typeof window.va === 'function') {
			window.va('racer3d_start', { track: this.trackId });
		}
	}

	update() {
		if (!this.running || !this.car) return;
		
		// Physics
		const maxSpeed = 15;
		const acceleration = 0.3;
		const deceleration = 0.15;
		const turnSpeed = 0.03;
		
		// Acceleration
		if (this.controls.forward) {
			this.speed = Math.min(this.speed + acceleration, maxSpeed);
		} else if (this.controls.backward) {
			this.speed = Math.max(this.speed - acceleration, -maxSpeed * 0.5);
		} else {
			this.speed *= 0.95; // Friction
		}
		
		// Steering (only when moving)
		if (Math.abs(this.speed) > 0.1) {
			if (this.controls.left) {
				this.car.rotation.y += turnSpeed * (this.speed / maxSpeed);
			}
			if (this.controls.right) {
				this.car.rotation.y -= turnSpeed * (this.speed / maxSpeed);
			}
		}
		
		// Drift mechanics
		const driftFactor = 0.85;
		const forward = new THREE.Vector3(0, 0, -1);
		forward.applyQuaternion(this.car.quaternion);
		
		this.car.position.x += forward.x * this.speed * driftFactor;
		this.car.position.z += forward.z * this.speed * driftFactor;
		
		// Keep car on ground
		this.car.position.y = 1;
		
		// Check checkpoints
		if (this.checkpoints.length > 0) {
			const checkpoint = this.checkpoints[this.currentCheckpoint % this.checkpoints.length];
			const dist = Math.sqrt((this.car.position.x - checkpoint.x)**2 + (this.car.position.z - checkpoint.z)**2);
			if (dist < 10) {
				this.currentCheckpoint++;
				if (this.currentCheckpoint >= this.checkpoints.length) {
					this.currentCheckpoint = 0;
					this.lapCount++;
					
					const lapTime = performance.now() - this.lapStart;
					this.lapStart = performance.now();
					
					if (this.lapCount >= this.maxLaps) {
						this.finishRace();
					} else {
						document.getElementById('r3d-lap').textContent = `${this.lapCount+1}/${this.maxLaps}`;
					}
				}
			}
		}
		
		// Update lap time
		this.currentLapTime = performance.now() - this.lapStart;
		document.getElementById('r3d-lap-time').textContent = this.formatTime(this.currentLapTime);
		
		// Update speed display
		document.getElementById('r3d-speed').textContent = Math.round(Math.abs(this.speed) * 10);
		
		// Camera follow
		const offset = new THREE.Vector3(0, 8, 15);
		offset.applyQuaternion(this.car.quaternion);
		this.camera.position.lerp(this.car.position.clone().add(offset), 0.1);
		this.camera.lookAt(this.car.position);
	}

	finishRace() {
		this.running = false;
		const totalTime = performance.now() - this.raceStart;
		this.saveTime(Math.round(totalTime));
		this.bestTime = this.getBestTime();
		document.getElementById('r3d-best-time').textContent = this.bestTime ? this.formatTime(this.bestTime) : '-';
		this.refreshLeaderboard();
		
		this.hideHUD();
		this.showOverlay('Race Complete!', `Total Time: ${this.formatTime(totalTime)}`);
		
		if (typeof window !== 'undefined' && typeof window.va === 'function') {
			window.va('racer3d_finish', { track: this.trackId, time: Math.round(totalTime) });
		}
	}

	animate() {
		requestAnimationFrame(() => this.animate());
		if (this.renderer && this.scene && this.camera) {
			this.update();
			this.renderer.render(this.scene, this.camera);
		}
	}

	formatTime(ms) {
		return (ms / 1000).toFixed(3);
	}

	showOverlay(title, msg) {
		const o = document.getElementById('racer3d-overlay');
		document.getElementById('r3d-overlay-title').textContent = title;
		if (msg) {
			document.getElementById('r3d-overlay-message').textContent = msg;
		}
		o.classList.remove('hidden');
	}
	
	hideOverlay() {
		document.getElementById('racer3d-overlay').classList.add('hidden');
	}
	
	showHUD() {
		document.getElementById('r3d-hud').style.display = 'block';
	}
	
	hideHUD() {
		document.getElementById('r3d-hud').style.display = 'none';
	}

	leaderboardKey() {
		return `racer3d_lb_${this.trackId}`;
	}

	getLeaderboard() {
		try {
			return JSON.parse(localStorage.getItem(this.leaderboardKey()) || '[]');
		} catch {
			return [];
		}
	}

	saveTime(ms) {
		const lb = this.getLeaderboard();
		if (lb.length >= 100 && ms >= lb[lb.length-1].ms) return;
		
		const name = prompt('New record! Enter your name:', 'Player');
		const entry = { name: (name || 'Player').slice(0, 16), ms };
		lb.push(entry);
		lb.sort((a, b) => a.ms - b.ms);
		const newLb = lb.slice(0, 100);
		localStorage.setItem(this.leaderboardKey(), JSON.stringify(newLb));
	}

	getBestTime() {
		const lb = this.getLeaderboard();
		return lb.length ? lb[0].ms : null;
	}

	refreshLeaderboard() {
		const list = document.getElementById('r3d-leaderboard');
		list.innerHTML = '';
		const lb = this.getLeaderboard();
		lb.forEach((e, idx) => {
			const li = document.createElement('li');
			li.textContent = `${idx + 1}. ${e.name} — ${this.formatTime(e.ms)}s`;
			list.appendChild(li);
		});
	}
}

let racer3D;
window.addEventListener('load', () => {
	if (document.getElementById('racer3d-container')) {
		racer3D = new Racer3D();
	}
});

