using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamikadze : BaseEnemyShip
{

    private StageShip _currentStage;
    
    private IEnumerator LookAtPlayer()
    {
        if (_player == null)
        {
            yield break;
        }
        while (_currentStage == StageShip.Wait)
        {
            Look(_player.transform.position - transform.position, true, true);
            //пропускаем кадр
            yield return null;
        }
    }

    protected override void UpdateStage(StageShip stage)
    {
        _currentStage = stage;

        switch (_currentStage)
        {
            case StageShip.In:
                break; 
            case StageShip.Wait:
                StartCoroutine(LookAtPlayer());
                break;
            case StageShip.Out:
                _playerLastPos = _player.transform.position;
                break;
        }
    }
}
