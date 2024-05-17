using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tony_Breakout
{
    // Base class representing a single block in the game
    public class Blocks : PictureBox
    {
        // Property to determine if the block has been hit
        public bool IsHit { get; set; }

        // Constructor to initialize a block with specific position
        public Blocks(int left, int top)
        {
            this.Height = 32; // Set H of block
            this.Width = 100; // Set W of block
            this.Tag = "blocks"; // Block tag
            this.BackColor = Color.White; // Initial background color of block
            this.Left = left; // X cords of block
            this.Top = top; // Y cords of block
            IsHit = false; // Block is initially not hit
        }

        // Method to mark the block as hit
        public void Hit()
        {
            IsHit = true;
        }
    }

    // Class representing a block that requires two hits to be destroyed
    public class DoubleHitBlock : Blocks
    {
        // Property to keep track of the number of hits remaining
        public int HitsRemaining { get; private set; }

        // Constructor to initialize DoubleHitBlock
        public DoubleHitBlock(int left, int top) : base(left, top)
        {
            HitsRemaining = 2; // Double hit block starts with 2 hits remaining
        }

        // Override Hit method to decrese the hits remaining
        public new void Hit()
        {
            HitsRemaining--; // Decrease number of hits remaining
            IsHit = true; // Mark the block as hit
        }

        // Method to check if the block is destroyed
        public bool IsDestroyed()
        {
            return HitsRemaining <= 0; // Returns true if no hits remaining
        }
    }

    // Main form class for the Breakout game
    public partial class Form1 : Form
    {
        // Boolean vars for player movement and gameover
        bool goLeft;
        bool goRight;
        bool isGameOver;


        // Int vars for Score, Ball and Player speed
        int score;
        int ballx;
        int bally;
        int playerSpeed;

        
        Random rnd = new Random(); // Random number generator for colors and ball speed
        PictureBox[] blockArray; // Array to hold all block objects

        // Constructor to initialize the form and place the blocks
        public Form1()
        {
            InitializeComponent(); // Initialize form components
            PlaceBlocks(); // Call method to place blocks on the form
        }

        // Method to set up initial game parameters and start the game
        private void setupGame()
        {
            isGameOver = false; // Set game over flag to false
            score = 0; // Initialize score 
            ballx = 5; // Set initial Hori speed of ball
            bally = 5; // Set initial Vert speed of ball
            playerSpeed = 12; // Set player speed
            txtScore.Text = "Score: " + score; // Update score display

            ball.Left = 376; // Set initial pos of ball: X
            ball.Top = 328; // Set initial pos of ball: Y

            player.Left = 347; // Set initial pos of player 

            gameTimer.Start(); // Start game timer

            // Randomize color of each block
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "blocks")
                {
                    x.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)); // Random color
                }
            }
        }

        // Method to handle game over scenario and display message
        private void gameOver(string message)
        {
            isGameOver = true; // Set game over flag to true
            gameTimer.Stop(); // Stop game timer

            txtScore.Text = "Score: " + score + " " + message; // Update score display with message
        }

        // Method to place blocks on the form
        private void PlaceBlocks()
        {
            blockArray = new PictureBox[15]; // Initialize array to hold 15 blocks

            int a = 0; // Counter of blocks in a row
            int top = 50; // Initial Top pos blocks
            int left = 100; // Initial L pos blocks

            for (int i = 0; i < blockArray.Length; i++)
            {
                blockArray[i] = new PictureBox(); // Create new PictureBox for each block
                blockArray[i].Height = 32; // Set block H
                blockArray[i].Width = 100; // Set block W

                // Every third block is a double hit block
                if (i % 3 == 0)
                {
                    blockArray[i] = new DoubleHitBlock(left, top);
                }
                else
                {
                    blockArray[i] = new Blocks(left, top);
                }

                // Adjust position for next row of blocks
                if (a == 5)
                {
                    top = top + 50; // Move to next row
                    left = 100; // Reset L pos
                    a = 0; // Reset counter
                }

                if (a < 5)
                {
                    a++; // Increment counter
                    blockArray[i].Left = left; // Set block L Pos
                    blockArray[i].Top = top; // Set block Top pos
                    this.Controls.Add(blockArray[i]); // Add block to form
                    left = left + 130; // Adjusts L pos for next block
                }
            }

            setupGame(); // Call setupGame method to initialize game parameters
        }

        // Method to remove blocks from the form
        private void removeBlocks()
        {
            foreach (PictureBox x in blockArray)
            {
                this.Controls.Remove(x); // Remove each block from the form
            }
        }

        // Event handler for the main game loop
        private void mainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score; // Update score display

            // Handle player movement to the left
            if (goLeft == true && player.Left > 0)
            {
                player.Left -= playerSpeed; // Move player left
            }

            // Handle player movement to the right
            if (goRight == true && player.Left < 700)
            {
                player.Left += playerSpeed; // Move player right
            }

            // Move the ball
            ball.Left += ballx; // horizontally
            ball.Top += bally; // vertically

            // Ball collision with left or right walls
            if (ball.Left < 0 || ball.Left > 755)
            {
                ballx = -ballx; // Reverse horizontal direction
            }

            // Ball collision with top wall
            if (ball.Top < 0)
            {
                bally = -bally; // Reverse vertical direction
            }

            // Ball collision with player paddle
            if (ball.Bounds.IntersectsWith(player.Bounds))
            {
                bally = rnd.Next(5, 12) * -1; // Bounce ball vertically

                // Randomize horizontal bounce direction
                if (ballx < 0)
                {
                    ballx = rnd.Next(5, 12) * -1; 
                }
                else
                {
                    ballx = rnd.Next(5, 12);
                }
            }

            // Ball collision with blocks
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "blocks")
                {
                    if (ball.Bounds.IntersectsWith(x.Bounds))
                    {

                        bally = -bally; // Bounce ball vertically

                        // Check if block is a double hit block
                        if (x is DoubleHitBlock doubleHitBlock)
                        {
                            doubleHitBlock.Hit(); // Hit the block
                            if (doubleHitBlock.IsDestroyed())
                            {
                                if (doubleHitBlock.IsHit)
                                {
                                    score += 1; 
                                    doubleHitBlock.IsHit = false; // Reset hit status
                                }
                                this.Controls.Remove(x); // Remove block 
                            }
                        }
                        else if (x is Blocks block) // Check if block is a regular block
                        {
                            block.Hit(); // Hit the block
                            if (block.IsHit)
                            {
                                score += 1; 
                                block.IsHit = false; // Reset hit status
                            }
                            this.Controls.Remove(x); // Remove block from form
                        }
                    }
                }
            }

            // Check for win condition
            if (score == 15)
            {
                gameOver("\nHaj Haj Haj, du van Tony Rickardsson:s Breakout spel!!\nKlicka \"Enter\" för att köra igen\nKlicka \"Escape\" för att avsluta");
            }

            // Check for lose condition
            if (ball.Top > 580)
            {
                gameOver("\nNaj Naj Naj, du förlora Tony Rickardsson:s Breakout Spel!!!\nKlicka \"Enter\" för att köra igen\nKlicka \"Escape\" för att avsluta");
            }
        }

        // Event handler for key down events (player movement)
        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true; // Start moving player left
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true; // Start moving player right
            }
        }

        // Event handler for key up events (player movement and game control)
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false; // Stop moving player left
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false; // Stop moving player right
            }
            if (e.KeyCode == Keys.Enter && isGameOver == true) // Resets game
            {
                removeBlocks(); // Remove existing blocks
                PlaceBlocks(); // Place new blocks and restart game
            }
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit(); // Exit 
            }
        }

        // Not in use
        private void player_Click(object sender, EventArgs e)
        {

        }

        // Not in use
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
