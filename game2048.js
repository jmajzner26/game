class Game2048 {
    constructor(){ this.c=document.getElementById('game2048-canvas'); this.x=this.c.getContext('2d'); this.size=4; this.grid=this.empty(); this.score=0; this.bind(); this.reset(); this.draw(); }
    empty(){ return Array.from({length:this.size},()=>Array(this.size).fill(0)); }
    reset(){ this.grid=this.empty(); this.addTile(); this.addTile(); this.showOverlay(); }
    bind(){ document.addEventListener('keydown',(e)=>{ const dirs=['ArrowLeft','ArrowRight','ArrowUp','ArrowDown']; if(e.code==='Space'){ this.start(); return; } if(!this.started) return; if(dirs.includes(e.key)){ e.preventDefault(); this.move(e.key); }});
        const btn=document.getElementById('game2048-start-btn'); if(btn) btn.addEventListener('click',()=>this.start()); }
    start(){ this.started=true; this.hideOverlay(); if(window.va) window.va('2048_start'); this.draw(); }
    addTile(){ const empt=[]; for(let r=0;r<this.size;r++) for(let c=0;c<this.size;c++) if(!this.grid[r][c]) empt.push([r,c]); if(!empt.length) return; const [r,c]=empt[Math.floor(Math.random()*empt.length)]; this.grid[r][c]=Math.random()<0.9?2:4; }
    slide(row){ const arr=row.filter(v=>v); for(let i=0;i<arr.length-1;i++){ if(arr[i]===arr[i+1]){ arr[i]*=2; this.score+=arr[i]; arr.splice(i+1,1); } } while(arr.length<this.size) arr.push(0); return arr; }
    rotateCW(g){ const n=this.size; const out=this.empty(); for(let r=0;r<n;r++) for(let c=0;c<n;c++) out[c][n-1-r]=g[r][c]; return out; }
    move(dir){ let g=this.grid; if(dir==='ArrowRight'){ g=g.map(r=>this.slide(r.reverse()).reverse()); }
        else if(dir==='ArrowLeft'){ g=g.map(r=>this.slide(r)); }
        else if(dir==='ArrowUp'){ g=this.rotateCW(this.rotateCW(this.rotateCW(g))); g=g.map(r=>this.slide(r)); g=this.rotateCW(g); }
        else if(dir==='ArrowDown'){ g=this.rotateCW(g); g=g.map(r=>this.slide(r)); g=this.rotateCW(this.rotateCW(this.rotateCW(g))); }
        if(JSON.stringify(g)!==JSON.stringify(this.grid)){ this.grid=g; this.addTile(); this.draw(); if(this.isGameOver()) this.end(); }
    }
    isGameOver(){ for(let r=0;r<this.size;r++) for(let c=0;c<this.size;c++){ if(!this.grid[r][c]) return false; const v=this.grid[r][c]; if(r+1<this.size && this.grid[r+1][c]===v) return false; if(c+1<this.size && this.grid[r][c+1]===v) return false; } return true; }
    draw(){ this.x.fillStyle='#bbada0'; this.x.fillRect(0,0,this.c.width,this.c.height); const s=this.c.width/this.size; for(let r=0;r<this.size;r++) for(let c=0;c<this.size;c++){ const v=this.grid[r][c]; this.x.fillStyle=v? '#eee4da':'#cdc1b4'; this.x.fillRect(c*s+6,r*s+6,s-12,s-12); if(v){ this.x.fillStyle='#776e65'; this.x.font='bold 24px Arial'; this.x.fillText(v,c*s+ s/2-10,r*s+ s/2+8); }} this.x.fillStyle='#fff'; this.x.font='16px Arial'; this.x.fillText(`Score: ${this.score}`,10,20); }
    end(){ this.started=false; this.showOverlay('Game Over'); if(window.va) window.va('2048_game_over',{score:this.score}); }
    showOverlay(msg){ const o=document.getElementById('game2048-overlay'); if(msg){ o.querySelector('h3').textContent=msg; } o.classList.remove('hidden'); }
    hideOverlay(){ const o=document.getElementById('game2048-overlay'); o.classList.add('hidden'); }
}
let g2048; window.addEventListener('load',()=>{ if(document.getElementById('game2048-canvas')) g2048=new Game2048(); });


