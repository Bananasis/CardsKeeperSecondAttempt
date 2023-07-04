using System;
using System.Collections.Generic;
using DefaultNamespace;
using UI.Menu;
using UnityEngine;
using Zenject;


public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GridMono _grid;
    [SerializeField] private TileHolder _tileHolder;
    [SerializeField] private RoomHolder _roomHolder;
    [SerializeField] private Hand _hand;
    [SerializeField] private CardPreview _preview;
    [SerializeField] private MoneyDisplay _money;
    [SerializeField] private WaveHolder _wave;
    [SerializeField] private WaveDisplay _waveDisplay;
    [SerializeField] private Recycler _recycler;
    [SerializeField] private FightManager _fightManager;
    [SerializeField] private MergeRoomsMenu _mergeRoomsMenu;
    [SerializeField] private MergeMobsMenu _mergeMobMenu;
    [SerializeField] private VFXHolder _vfxHolder;
    [SerializeField] private StoreMenu _storeMenu;
    [SerializeField] private TurnOrder _turnOrder;
    public override void InstallBindings()
    {
        Container.Bind<TurnOrder>().To<TurnOrder>().FromInstance(_turnOrder).AsSingle();
        Container.Bind<StoreMenu>().To<StoreMenu>().FromInstance(_storeMenu).AsSingle();
        Container.Bind<VFXHolder>().To<VFXHolder>().FromInstance(_vfxHolder).AsSingle();
        Container.Bind<MergeMobsMenu>().To<MergeMobsMenu>().FromInstance(_mergeMobMenu).AsSingle();
        Container.Bind<GameManager>().To<GameManager>().FromInstance(_gameManager).AsSingle();
        Container.Bind<MergeRoomsMenu>().To<MergeRoomsMenu>().FromInstance(_mergeRoomsMenu).AsSingle();
        Container.Bind<GridMono>().To<GridMono>().FromInstance(_grid).AsSingle();
        Container.Bind<TileHolder>().To<TileHolder>().FromInstance(_tileHolder).AsSingle();
        Container.Bind<RoomHolder>().To<RoomHolder>().FromInstance(_roomHolder).AsSingle();
        Container.Bind<WaveHolder>().To<WaveHolder>().FromInstance(_wave).AsSingle();
        Container.Bind<MoneyDisplay>().To<MoneyDisplay>().FromInstance(_money).AsSingle();
        Container.Bind<WaveDisplay>().To<WaveDisplay>().FromInstance(_waveDisplay).AsSingle();
        Container.Bind<Hand>().To<Hand>().FromInstance(_hand).AsCached();
        Container.Bind<CardPreview>().To<CardPreview>().FromInstance(_preview).AsSingle();
        Container.Bind<GridData>().To<GridData>().FromInstance(new GridData()).AsSingle();
        Container.Bind<SaveData>().To<SaveData>().AsSingle();
        Container.Bind<FightManager>().To<FightManager>().FromInstance(_fightManager).AsSingle();
        Container.Bind<IRecycler>().To<IRecycler>().FromInstance(_recycler).AsSingle();
    }
}