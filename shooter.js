class ShooterGame {
    constructor(){ this.c=document.getElementById('shooter-canvas'); this.x=this.c.getContext('2d'); this.bind(); this.reset(); this.draw(); }
    reset(){ this.player={x:this.c.width/2,y:this.c.height-40, w:20,h:20, vx:0}; this.bullets=[]; this.enemies=[]; this.t=0; this.running=false; this.showOverlay(); }
    bind(){ document.addEventListener('keydown',(e)=>{ if(e.code==='Space'&&!this.running){ this.start(); return; } if(!this.running) return; if(e.key==='ArrowLeft') this.player.vx=-5; if(e.key==='ArrowRight') this.player.vx=5; if(e.code==='Space') this.fire(); });
        document.addEventListener('keyup',(e)=>{ if(e.key==='ArrowLeft'||e.key==='ArrowRight') this.player.vx=0; }); const btn=document.getElementById('shooter-start-btn'); if(btn) btn.addEventListener('click',()=>this.start()); }
    start(){ this.running=true; this.hideOverlay(); if(window.va) window.va('shooter_start'); requestAnimationFrame(()=>this.loop()); }
    loop(){ if(!this.running) return; this.update(); this.draw(); requestAnimationFrame(()=>this.loop()); }
    update(){ this.t++; this.player.x+=this.player.vx; this.player.x=Math.max(0,Math.min(this.c.width-this.player.w,this.player.x)); if(this.t%50===0){ this.enemies.push({x:Math.random()*(this.c.width-20),y:-20,w:20,h:20,vy:2}); }
        this.enemies.forEach(e=>e.y+=e.vy); this.enemies=this.enemies.filter(e=>e.y<this.c.height+40);
        this.bullets.forEach(b=>b.y-=6); this.bullets=this.bullets.filter(b=>b.y>-10);
        // collisions
        for(const e of this.enemies){ for(const b of this.bullets){ if(b.x>e.x&&b.x<e.x+e.w&&b.y>e.y&&b.y<e.y+e.h){ e.hit=true; b.hit=true; } } if(e.y+e.h>this.player.y && e.x<this.player.x+this.player.w && e.x+e.w>this.player.x){ this.end(); return; } }
        this.enemies=this.enemies.filter(e=>!e.hit); this.bullets=this.bullets.filter(b=>!b.hit);
    }
    fire(){ this.bullets.push({x:this.player.x+this.player.w/2,y:this.player.y}); }
    draw(){ this.x.fillStyle='#000'; this.x.fillRect(0,0,this.c.width,this.c.height); this.x.fillStyle='#4ecdc4'; this.x.fillRect(this.player.x,this.player.y,this.player.w,this.player.h); this.x.fillStyle='#fff'; for(const b of this.bullets) this.x.fillRect(b.x-2,b.y-8,4,8); this.x.fillStyle='#ff6b6b'; for(const e of this.enemies) this.x.fillRect(e.x,e.y,e.w,e.h); }
    showOverlay(){ document.getElementById('shooter-overlay').classList.remove('hidden'); }
    hideOverlay(){ document.getElementById('shooter-overlay').classList.add('hidden'); }
    end(){ this.running=false; this.showOverlay(); if(window.va) window.va('shooter_game_over'); }
}
let shooterGame; window.addEventListener('load',()=>{ if(document.getElementById('shooter-canvas')) shooterGame=new ShooterGame(); });


