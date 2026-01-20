using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ZeroElectric.Vinculum;
using TurboMapReader;
using RayGuiCreator;

namespace Rogue
{
    /// <summary>
    /// Pääpeliluokka, joka hallinnoi pelitiloja, pelaajaa, karttaa ja valikoita.
    /// </summary>
    internal class Peli
    {
        private PlayerCharacter _player;
        private readonly TextBoxEntry _playerNameBox = new(12);
        private Kartta _firstLevel;

        private readonly MultipleChoiceEntry _raceSelector =
            new(new[] { "Piru", "Tonttu", "Hobitti" });
        private readonly MultipleChoiceEntry _classSelector =
            new(new[] { "Megaknigt", "Koira", "Kissa" });

        public enum GameState
        {
            MainMenu,
            CharacterCreation,
            GameLoop,
            PauseMenu,
            OptionsMenu,
            Quit
        }

        private readonly Stack<GameState> _stateStack = new();
        private OptionsMenu _optionsMenu;
        private Paussimenu _pauseMenu;
        private GameState _activeState;
        private Vihollinen _enemy;
        private Sound _stepSound;

        public static readonly int TileSize = 16;
        public static readonly int tileSize = TileSize;

        private const int WIDTH = 1280;
        private const int HEIGHT = 720;

        /// <summary>
        /// Aloittaa pelin: luo valikot, pelaajan, kartan ja käynnistää pelisilmukan.
        /// </summary>
        public void Run()
        {
            _stateStack.Push(GameState.MainMenu);

            _optionsMenu = new OptionsMenu();
            _pauseMenu = new Paussimenu();

            _optionsMenu.BackButtonPressedEvent += (_, _) => _stateStack.Pop();
            _pauseMenu.BackButtonPressedEvent += HandlePauseMenuReturn;

            Console.CursorVisible = false;
            Console.SetWindowSize(60, 26);

            _player = new PlayerCharacter('b', Raylib.GREEN, ConsoleColor.Green)
            {
                position = new Vector2(3, 3)
            };

            var mapReader = new Kartanlukija();
            _firstLevel = mapReader.ReadTiledMapFromFile("tiled/Rogue.tmj");

            Raylib.InitWindow(WIDTH, HEIGHT, "Raylib");
            Raylib.InitAudioDevice();

            _stepSound = Raylib.LoadSound("C:\\dotnetantonfinal\\Rogue\\Sound\\Liikkuminen.mp3");
            Texture atlas = Raylib.LoadTexture("Images/tilemap.png");

            _firstLevel.SetImageAndIndex(atlas, 5, 50);
            _firstLevel.LoadEnemiesAndItems();
            _player.SetImageAndIndex(atlas, 5, 50);

            CoreLoop();
            Raylib.CloseWindow();
        }

        /// <summary>
        /// Piirtää päävalikon ja käsittelee valikon napit.
        /// </summary>
        private void DrawMainMenu()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.BLACK);

            int bw = 500, bh = 50;
            int x = Raylib.GetScreenWidth() / 2 - bw / 2;
            int y = Raylib.GetScreenHeight() / 2 - bh / 2;

            RayGui.GuiLabel(new Rectangle(x, y - bh * 2, bw, bh), "Peli");

            if (RayGui.GuiButton(new Rectangle(x, y, bw, bh), "Aloita") == 1)
                _stateStack.Push(GameState.CharacterCreation);

            y += bh * 2;
            if (RayGui.GuiButton(new Rectangle(x, y, bw, bh), "Asetukset") == 1)
                _stateStack.Push(GameState.OptionsMenu);

            y += bh * 2;
            if (RayGui.GuiButton(new Rectangle(x, y, bw, bh), "Paussi") == 1)
                _stateStack.Push(GameState.PauseMenu);

            y += bh * 2;
            if (RayGui.GuiButton(new Rectangle(x, y, bw, bh), "Lopeta") == 1)
                _stateStack.Push(GameState.Quit);

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Piirtää hahmonluontivalikon ja käsittelee valinnat.
        /// </summary>
        /// <param name="x">Valikon X-koordinaatti</param>
        /// <param name="y">Valikon Y-koordinaatti</param>
        /// <param name="width">Valikon leveys</param>
        private void DrawCharacterMenu(int x, int y, int width)
        {
            Raylib.ClearBackground(
                Raylib.GetColor((uint)RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.BACKGROUND_COLOR)));

            var menu = new MenuCreator(x, y, Raylib.GetScreenHeight() / 20, width);

            menu.Label("Luo hahmo");
            menu.Label("Hahmon nimi");
            menu.TextBox(_playerNameBox);

            menu.Label("Luokka");
            menu.DropDown(_classSelector);

            menu.Label("Rotu");
            menu.DropDown(_raceSelector);

            if (menu.Button("Aloita"))
            {
                TryCreateCharacter();
            }

            menu.EndMenu();
            Raylib.EndDrawing();
        }

        /// <summary>
        /// Pelin pääsilmukka, joka päivittää tilat, valikot ja pelilogiiikan.
        /// </summary>
        private void CoreLoop()
        {
            bool active = true;

            while (!Raylib.WindowShouldClose() && active)
            {
                switch (_stateStack.Peek())
                {
                    case GameState.MainMenu: DrawMainMenu(); break;
                    case GameState.CharacterCreation: DrawCharacterMenu(Raylib.GetScreenWidth() / 2 - 150, 40, 300); break;
                    case GameState.PauseMenu: _pauseMenu.DrawMenu(); break;
                    case GameState.OptionsMenu: _optionsMenu.DrawMenu(); break;
                    case GameState.GameLoop:
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Raylib.BLACK);
                        Raylib.EndDrawing();
                        HandleInput();
                        RedrawGame();
                        break;
                    case GameState.Quit: active = false; break;
                }
            }
        }

        /// <summary>
        /// Piirtää pelin sisällön konsoliin ja Raylibiin.
        /// </summary>
        private void RedrawGame()
        {
            Console.Clear();
            _firstLevel.DrawMap();
            _player.Draw();
        }

        /// <summary>
        /// Käsittelee paussivalikon paluun ja pelin jatkamisen.
        /// </summary>
        private void HandlePauseMenuReturn(object sender, GameState next)
        {
            if (next == GameState.GameLoop)
                _stateStack.Pop();
            else
                _stateStack.Push(GameState.MainMenu);
        }

        /// <summary>
        /// Tarkistaa pelaajan syötteen ja luo hahmon, jos valinnat ovat validit.
        /// </summary>
        private void TryCreateCharacter()
        {
            string name = _playerNameBox.ToString();
            bool validName = !string.IsNullOrEmpty(name) && name.All(char.IsLetter);

            bool raceOk = false;
            bool classOk = false;

            switch (_raceSelector.ToString())
            {
                case "Piru": _player.rotu = Hahmot.Piru; raceOk = true; break;
                case "Tonttu": _player.rotu = Hahmot.Tonttu; raceOk = true; break;
                case "Hobitti": _player.rotu = Hahmot.Hobitti; raceOk = true; break;
            }

            switch (_classSelector.ToString())
            {
                case "Megaknigt": _player.hahmoluokka = Class.Megaknigt; classOk = true; break;
                case "Koira": _player.hahmoluokka = Class.Koira; classOk = true; break;
                case "Kissa": _player.hahmoluokka = Class.Kissa; classOk = true; break;
            }

            if (validName && raceOk && classOk)
            {
                _activeState = GameState.GameLoop;
                _stateStack.Push(_activeState);
            }
        }

        /// <summary>
        /// Käsittelee pelaajan liikkeet näppäimistön mukaan ja estää liikkumisen seinien läpi.
        /// </summary>
        private void HandleInput()
        {
            Vector2 p = _player.position;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { p.Y -= 1; Raylib.PlaySound(_stepSound); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { p.Y += 1; Raylib.PlaySound(_stepSound); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) { p.X -= 1; Raylib.PlaySound(_stepSound); }
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) { p.X += 1; Raylib.PlaySound(_stepSound); }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_TAB)) _stateStack.Push(GameState.PauseMenu);

            if (_firstLevel.GetTileAtGround((int)p.X, (int)p.Y) == Kartta.MapTile.Floor)
                _player.position = p;
        }
    }
}
