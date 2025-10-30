class FlappyGame {
    constructor(){ this.c=document.getElementById('flappy-canvas'); this.x=this.c.getContext('2d'); this.reset(); this.bind(); this.draw(); }
    reset(){ this.bird={y:this.c.height/2, vy:0}; this.pipes=[]; this.t=0; this.running=false; this.score=0; this.showOverlay(); }
    bind(){ document.addEventListener('keydown',(e)=>{ if(e.code==='Space'){ if(!this.running){ this.start(); } else { this.bird.vy=-7; } }});
        this.c.addEventListener('click',()=>{ if(!this.running) this.start(); else this.bird.vy=-7; });
        const btn=document.getElementById('flappy-start-btn'); if(btn) btn.addEventListener('click',()=>this.start()); }
    start(){ this.running=true; this.hideOverlay(); if(window.va) window.va('flappy_start'); requestAnimationFrame(()=>this.loop()); }
    loop(){ if(!this.running) return; this.update(); this.draw(); requestAnimationFrame(()=>this.loop()); }
    update(){ this.t++; if(this.t%90===0){ const gap=140; const top=Math.random()* (this.c.height-gap-80)+40; this.pipes.push({x:this.c.width,top:top,gap:gap}); }
        this.pipes.forEach(p=>p.x-=3); this.pipes=this.pipes.filter(p=>p.x>-60);
        this.bird.vy+=0.4; this.bird.y+=this.bird.vy; if(this.bird.y<0||this.bird.y>this.c.height) return this.end();
        // collision and score
        for(const p of this.pipes){ if(p.x<60&&p.x>0){ if(this.bird.y<p.top || this.bird.y>p.top+p.gap) return this.end(); }
            if(p.x===60) this.score++; }
    }
    draw(){ this.x.fillStyle='#70c5ce'; this.x.fillRect(0,0,this.c.width,this.c.height);
        // pipes
        this.x.fillStyle='#5cb85c'; for(const p of this.pipes){ this.x.fillRect(p.x,0,60,p.top); this.x.fillRect(p.x,p.top+p.gap,60,this.c.height-(p.top+p.gap)); }
        // bird
        this.x.fillStyle='#ffec99'; this.x.beginPath(); this.x.arc(60,this.bird.y,12,0,Math.PI*2); this.x.fill();
        // score
        this.x.fillStyle='#fff'; this.x.font='20px Arial'; this.x.fillText(this.score,10,30);
    }
    end(){ this.running=false; this.showOverlay('Game Over'); if(window.va) window.va('flappy_game_over',{score:this.score}); }
    showOverlay(msg){ const o=document.getElementById('flappy-overlay'); if(msg){ o.querySelector('h3').textContent=msg; } o.classList.remove('hidden'); }
    hideOverlay(){ const o=document.getElementById('flappy-overlay'); o.classList.add('hidden'); }
}
let flappyGame; window.addEventListener('load',()=>{ if(document.getElementById('flappy-canvas')) flappyGame=new FlappyGame(); });


