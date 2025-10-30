class WhackGame {
    constructor(){ this.c=document.getElementById('whack-canvas'); this.x=this.c.getContext('2d'); this.size=3; this.moles=[]; this.score=0; this.running=false; this.bind(); this.layout(); this.draw(); this.showOverlay(); }
    layout(){ this.holes=[]; const s=this.c.width/this.size; for(let r=0;r<this.size;r++) for(let c=0;c<this.size;c++){ this.holes.push({x:c*s+s/2,y:r*s+s/2}); } }
    bind(){ this.c.addEventListener('click',(e)=>{ if(!this.running){ this.start(); return; } const rect=this.c.getBoundingClientRect(); const mx=e.clientX-rect.left, my=e.clientY-rect.top; for(const m of this.moles){ const dx=mx-m.x, dy=my-m.y; if(Math.hypot(dx,dy)<25){ this.score++; m.hit=true; } } }); const btn=document.getElementById('whack-start-btn'); if(btn) btn.addEventListener('click',()=>this.start()); }
    start(){ this.running=true; this.score=0; if(window.va) window.va('whack_start'); this.moles=[]; this.nextPop(); this.hideOverlay(); this.loop(); }
    nextPop(){ if(!this.running) return; const idx=Math.floor(Math.random()*this.holes.length); const h=this.holes[idx]; this.moles.push({x:h.x,y:h.y, t:Date.now(), hit:false}); setTimeout(()=>this.nextPop(), 700); }
    loop(){ if(!this.running) return; this.update(); this.draw(); setTimeout(()=>this.loop(), 30); }
    update(){ const now=Date.now(); this.moles=this.moles.filter(m=> now-m.t<800 && !m.hit ); }
    draw(){ this.x.fillStyle='#3c873a'; this.x.fillRect(0,0,this.c.width,this.c.height); this.x.fillStyle='#462'; for(const h of this.holes){ this.x.beginPath(); this.x.arc(h.x,h.y,30,0,Math.PI*2); this.x.fill(); }
        this.x.fillStyle='#ffeb3b'; for(const m of this.moles){ this.x.beginPath(); this.x.arc(m.x,m.y,25,0,Math.PI*2); this.x.fill(); }
        this.x.fillStyle='#fff'; this.x.font='16px Arial'; this.x.fillText(`Score: ${this.score}`,10,20);
    }
    showOverlay(){ document.getElementById('whack-overlay').classList.remove('hidden'); }
    hideOverlay(){ document.getElementById('whack-overlay').classList.add('hidden'); }
}
let whackGame; window.addEventListener('load',()=>{ if(document.getElementById('whack-canvas')) whackGame=new WhackGame(); });


