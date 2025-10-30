class MemoryGame {
    constructor(){ this.grid=document.getElementById('memory-grid'); this.size=8; this.cards=[]; this.flipped=[]; this.matched=0; this.running=false; this.setup(); this.showOverlay(); }
    setup(){ const icons=['🍎','🍌','🍇','🍉','🍒','🍑','🍍','🥝']; const deck=[...icons,...icons].sort(()=>Math.random()-0.5); this.grid.innerHTML=''; this.cards=[]; deck.forEach((val,i)=>{ const el=document.createElement('button'); el.setAttribute('style','width:80px;height:80px;border-radius:8px;background:#222;color:#222;font-size:28px;font-weight:bold;cursor:pointer;border:1px solid #444;'); el.textContent=val; el.dataset.value=val; el.dataset.revealed='false'; el.addEventListener('click',()=>this.click(el)); this.grid.appendChild(el); this.cards.push(el); }); }
    start(){ this.running=true; document.getElementById('memory-overlay').classList.add('hidden'); if(window.va) window.va('memory_start'); this.hideAll(); }
    hideAll(){ this.cards.forEach(c=>{ c.dataset.revealed='false'; c.style.color='#222'; }); }
    click(el){ if(!this.running) return; if(el.dataset.revealed==='true') return; el.dataset.revealed='true'; el.style.color='#fff'; this.flipped.push(el); if(this.flipped.length===2){ const [a,b]=this.flipped; if(a.dataset.value===b.dataset.value){ this.matched+=2; this.flipped=[]; if(this.matched===this.cards.length) this.win(); } else { setTimeout(()=>{ a.dataset.revealed='false'; b.dataset.revealed='false'; a.style.color='#222'; b.style.color='#222'; this.flipped=[]; },600); } } }
    win(){ this.running=false; const o=document.getElementById('memory-overlay'); o.querySelector('h3').textContent='You Win!'; o.classList.remove('hidden'); if(window.va) window.va('memory_win'); }
}
let memoryGame; window.addEventListener('load',()=>{ const start=document.getElementById('memory-start-btn'); if(start){ memoryGame=new MemoryGame(); start.addEventListener('click',()=>memoryGame.start()); }});


