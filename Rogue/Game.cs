using System;
using System.Collections.Generic;
using System.Numerics;
using ZeroElectric.Vinculum;
using TurboMapReader;
using RayGuiCreator;

namespace Rogue
{
    internal class Game
    {
        // Pelaaja
        private PlayerCharacter player;

        // Pelaajan nimi (tekstikenttä)
        private TextBoxEntry playerNameEntry = new(10);

        // Kartta
        private Map lvl1;

        // Rotuvalikko
        MultipleChoiceEntry race = new MultipleChoiceEntry(
            new string[] { "Human", "Elf", "Dwarf" });

        // Hahmoluokkavalikko
        MultipleChoiceEntry characterClass = new MultipleChoiceEntry(
            new string[] { "Warrior", "Mage", "Rogue" });

        // Pelitilan enum
        public enum GameState
        {
            MainMenu,
            CharacterCreation,
            GameLoop,
            PauseMenu,
            OptionsMenu,
            Quit
        }

        // Pelitilan pino (mahdollistaa takaisin siirtymisen)
        private Stack<GameState> stateStack = new();

        // Valikot
        private OptionsMenu myOptionsMenu;
        private PauseMenu myPauseMenu;

        // Nykyinen tila (varmistaa piirtämisen logiikassa)
        private GameState currentGameState;

        // Yksi vihollinen (tähän voisi lisätä listan useita vihollisia varten)
        private Enemy enemy;

        // Ääniefekti
        private Sound soundToPlay;

        // Tiilikoko (sprite resoluutio)
        public static readonly int tileSize = 16;

        // Ikkunan koko
        private const int screen_width = 1280;
        private const int screen_height = 720;

        public void Run()
        {
            // Aloitetaan päävalikosta
            stateStack.Push(GameState.MainMenu);

            // Luodaan valikot ja asetetaan paluunapit
            myOptionsMenu = new OptionsMenu();
            myPauseMenu = new PauseMenu();

            myOptionsMenu.BackButtonPressedEvent += OnOptionsBackButtonPressed;
            myPauseMenu.BackButtonPressedEvent += OnPauseBackButtonPressed;

            // Konsoli-setup
            Console.CursorVisible = false;
            Console.WindowWidth = 60;
            Console.WindowHeight = 26;

            // Luodaan pelaaja
            player = new PlayerCharacter('@', Raylib.RED, ConsoleColor.Green);
            player.position = new Vector2(3, 3);

            // Luetaan kartta TMJ-tiedostosta
            MapReader reader = new();
            lvl1 = reader.ReadTiledMapFromFile("tiled/Rogue.tmj");

            // Alustetaan Raylib
            Raylib.InitWindow(screen_width, screen_height, "Raylib");
            Raylib.InitAudioDevice();
            soundToPlay = Raylib.LoadSound("Sound/click.mp3");
            Texture imageTexture = Raylib.LoadTexture("Images/tilemap.png");

            // Asetetaan kuvat kartalle ja pelaajalle
            lvl1.SetImageAndIndex(imageTexture, 12, 109);
            lvl1.LoadEnemiesAndItems();
            player.SetImageAndIndex(imageTexture, 12, 109);

            // Käynnistetään peli
            GameLoop();

            Raylib.CloseWindow();
        }

        private void DrawMainMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            int button_width = 100;
            int button_height = 20;
            int button_x = Raylib.GetScreenWidth() / 2 - button_width / 2;
            int button_y = Raylib.GetScreenHeight() / 2 - button_height / 2;

            RayGui.GuiLabel(new Rectangle(button_x, button_y - button_height * 2, button_width, button_height), "Rogue");

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Start Game") == 1)
                stateStack.Push(GameState.CharacterCreation);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Options") == 1)
                stateStack.Push(GameState.OptionsMenu);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Pause") == 1)
                stateStack.Push(GameState.PauseMenu);

            button_y += button_height * 2;

            if (RayGui.GuiButton(new Rectangle(button_x, button_y, button_width, button_height), "Quit") == 1)
                stateStack.Push(GameState.Quit);

            Raylib.EndDrawing();
        }

        private void DrawRay()
        {
            // Placeholder-ruutu tyhjälle peliloopille
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);
            Raylib.EndDrawing();
        }

        // Takaisin-napit valikoissa
        private void OnOptionsBackButtonPressed(object sender, EventArgs args) => stateStack.Pop();

        private void OnPauseBackButtonPressed(object sender, GameState newState)
        {
            if (newState == GameState.GameLoop)
                stateStack.Pop();
            else
                stateStack.Push(GameState.MainMenu);
        }

        private void GameLoop()
        {
            bool gameIsOn = true;

            while (!Raylib.WindowShouldClose() && gameIsOn)
            {
                switch (stateStack.Peek())
                {
                    case GameState.MainMenu:
                        DrawMainMenu();
                        break;
                    case GameState.CharacterCreation:
                        DrawCharacterMenu(Raylib.GetScreenWidth() / 2 - 150, 40, 300);
                        break;
                    case GameState.PauseMenu:
                        myPauseMenu.DrawMenu();
                        break;
                    case GameState.OptionsMenu:
                        myOptionsMenu.DrawMenu();
                        break;
                    case GameState.GameLoop:
                        DrawRay();   // Placeholder-tausta
                        Update();    // Pelaajan liikkuminen
                        Draw();      // Kartta ja pelaaja
                        break;
                    case GameState.Quit:
                        gameIsOn = false;
                        break;
                }
            }
        }

        private void Draw()
        {
            Console.Clear();
            lvl1.DrawMap();
            player.Draw();
        }

        private void DrawCharacterMenu(int x, int y, int width)
        {
            Raylib.ClearBackground(
                Raylib.GetColor((uint)RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.BACKGROUND_COLOR))
            );

            // Käytetään RayGuiCreatorin MenuCreator-luokkaa
            MenuCreator c = new(x, y, Raylib.GetScreenHeight() / 20, width);
            c.Label("Create Character");
            c.Label("Name Character");
            c.TextBox(playerNameEntry);
            c.Label("Select Class");
            c.DropDown(characterClass);
            c.Label("Select Race");
            c.DropDown(race);

            if (c.Button("Start Game"))
            {
                string nimi = playerNameEntry.ToString();
                bool nameOk = !string.IsNullOrEmpty(nimi) && nimi.All(char.IsLetter);

                bool raceSelect = false;
                bool classSelect = false;

                // Rotuvalinta
                switch (race.ToString())
                {
                    case "Human":
                        player.rotu = Race.Human;
                        raceSelect = true;
                        break;
                    case "Elf":
                        player.rotu = Race.Elf;
                        raceSelect = true;
                        break;
                    case "Dwarf":
                        player.rotu = Race.Dwarf;
                        raceSelect = true;
                        break;
                }

                // Luokkavalinta
                switch (characterClass.ToString())
                {
                    case "Warrior":
                        player.hahmoluokka = Class.Warrior;
                        classSelect = true;
                        break;
                    case "Mage":
                        player.hahmoluokka = Class.Mage;
                        classSelect = true;
                        break;
                    case "Rogue":
                        player.hahmoluokka = Class.Rogue;
                        classSelect = true;
                        break;
                }

                // Jos kaikki valinnat ovat kunnossa -> peliin
                if (nameOk && raceSelect && classSelect)
                    stateStack.Push(currentGameState = GameState.GameLoop);
            }

            c.EndMenu();
            Raylib.EndDrawing();
        }

        private void Update()
        {
            // Pelaajan liikkumislogiikka ja ääni
            Vector2 newPlace = player.position;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { newPlace.Y -= 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { newPlace.Y += 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) { newPlace.X -= 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) { newPlace.X += 1; Raylib.PlaySound(soundToPlay); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB)) stateStack.Push(GameState.PauseMenu);

            // Tarkistaa onko kohderuutu lattiaa ennen siirtoa
            if (lvl1.GetTileAtGround((int)newPlace.X, (int)newPlace.Y) == Map.MapTile.Floor)
                player.position = newPlace;
        }
    }
}
