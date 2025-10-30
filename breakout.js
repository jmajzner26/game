class BreakoutGame {
    constructor(){
        this.canvas=document.getElementById('breakout-canvas');
        this.ctx=this.canvas.getContext('2d');
        this.bind(); this.reset(); this.draw();
    }
    reset(){
        this.paddle={ w:80,h:12,x:this.canvas.width/2-40,y:this.canvas.height-20, vx:0 };
        this.ball={ x:this.canvas.width/2,y:this.canvas.height-40,vx:3,vy:-3,r:7 };
        this.rows=5; this.cols=10; this.brickW=(this.canvas.width-40)/this.cols; this.brickH=18;
        this.bricks=[]; for(let r=0;r<this.rows;r++){this.bricks[r]=[];for(let c=0;c<this.cols;c++){this.bricks[r][c]=1;}}
        this.running=false; this.score=0;
        this.showOverlay();
    }
    bind(){
        document.addEventListener('keydown',(e)=>{ if(e.code==='Space'&&!this.running){this.start();}
            if(!this.running) return; if(e.key==='ArrowLeft') this.paddle.vx=-6; if(e.key==='ArrowRight') this.paddle.vx=6; });
        document.addEventListener('keyup',(e)=>{ if(e.key==='ArrowLeft'||e.key==='ArrowRight') this.paddle.vx=0; });
        const btn=document.getElementById('breakout-start-btn'); if(btn) btn.addEventListener('click',()=>this.start());
    }
    start(){ this.running=true; this.hideOverlay(); if(window.va) window.va('breakout_start'); requestAnimationFrame(()=>this.loop()); }
    loop(){ if(!this.running) return; this.update(); this.draw(); requestAnimationFrame(()=>this.loop()); }
    update(){
        this.paddle.x+=this.paddle.vx; this.paddle.x=Math.max(0,Math.min(this.canvas.width-this.paddle.w,this.paddle.x));
        this.ball.x+=this.ball.vx; this.ball.y+=this.ball.vy;
        if(this.ball.x<this.ball.r||this.ball.x>this.canvas.width-this.ball.r) this.ball.vx*=-1;
        if(this.ball.y<this.ball.r) this.ball.vy*=-1;
        if(this.ball.y>this.canvas.height){ this.end(); return; }
        // paddle
        if(this.ball.y+this.ball.r>this.paddle.y && this.ball.x>this.paddle.x && this.ball.x<this.paddle.x+this.paddle.w){
            this.ball.vy=-Math.abs(this.ball.vy); const hit=((this.ball.x-(this.paddle.x+this.paddle.w/2))/this.paddle.w); this.ball.vx=5*hit; }
        // bricks
        for(let r=0;r<this.rows;r++) for(let c=0;c<this.cols;c++) if(this.bricks[r][c]){
            const bx=20+c*this.brickW, by=40+r*this.brickH;
            if(this.ball.x>bx && this.ball.x<bx+this.brickW && this.ball.y>by && this.ball.y<by+this.brickH){
                this.bricks[r][c]=0; this.ball.vy*=-1; this.score+=1; if(this.score===this.rows*this.cols) this.win();
            }
        }
    }
    draw(){
        this.ctx.fillStyle='#000'; this.ctx.fillRect(0,0,this.canvas.width,this.canvas.height);
        // paddle
        this.ctx.fillStyle='#4ecdc4'; this.ctx.fillRect(this.paddle.x,this.paddle.y,this.paddle.w,this.paddle.h);
        // ball
        this.ctx.fillStyle='#fff'; this.ctx.beginPath(); this.ctx.arc(this.ball.x,this.ball.y,this.ball.r,0,Math.PI*2); this.ctx.fill();
        // bricks
        for(let r=0;r<this.rows;r++) for(let c=0;c<this.cols;c++) if(this.bricks[r][c]){
            const bx=20+c*this.brickW, by=40+r*this.brickH; this.ctx.fillStyle='#ff6b6b'; this.ctx.fillRect(bx,by,this.brickW-2,this.brickH-2);
        }
        // score
        this.ctx.fillStyle='#fff'; this.ctx.font='16px Arial'; this.ctx.fillText(`Score: ${this.score}`,10,20);
    }
    end(){ this.running=false; this.showOverlay('Game Over'); if(window.va) window.va('breakout_game_over',{score:this.score}); }
    win(){ this.running=false; this.showOverlay('You Win!'); if(window.va) window.va('breakout_win',{score:this.score}); }
    showOverlay(msg){ const o=document.getElementById('breakout-overlay'); if(msg){ o.querySelector('h3').textContent=msg; } o.classList.remove('hidden'); }
    hideOverlay(){ const o=document.getElementById('breakout-overlay'); o.classList.add('hidden'); }
}
let breakoutGame; window.addEventListener('load',()=>{ if(document.getElementById('breakout-canvas')) breakoutGame=new BreakoutGame(); });


