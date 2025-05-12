using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// La hacemos static para que no dependa de la instancia de un objeto. No se puede creear obj de esta clase.
// Como todo en esta clase va a ser static, hacemos a la clase tmb static, aunque no hace falta.
public static class Loader
{
    public enum Scene{
        Menu,
        CharacterSelect,
        TestDemo,
        TestDemo1,
        WorldSelect,
        WorldSelectTesting,
        Loading,
        Level2
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene) {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.Loading.ToString());
        // Tenemos que esperar al menos 1 frame entre el render de la escena y el proximo call o sino no se ver[ia la escena de carga.
    }
    
    // Overload para recargar una escena.
    public static void Load(string targetSceneName) {
        Scene targetScene;

        if(TryGetSceneName(targetSceneName, out targetScene)) {
            Loader.targetScene = targetScene;
        } else {
            Debug.LogError("NO EXISTE UNA ESCENA CON NOMBRE: " + targetSceneName);
        }

        SceneManager.LoadScene(Scene.Loading.ToString());
        // Tenemos que esperar al menos 1 frame entre el render de la escena y el proximo call o sino no se ver[ia la escena de carga.
    }

    // Se triggerea en el 1er update de la escena, y carga la escena posta.
    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }

    // Tratamos de sacar si hay un enum con el mismo nombre.
    static bool TryGetSceneName(string value, out Scene result)
    {
        return Enum.TryParse(value, true, out result);
    }

}