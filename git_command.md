Opción 1: Desde la Terminal (Más rápido y seguro)

Abre la terminal integrada en Visual Studio o VS Code (`Ctrl + ñ` o `Ctrl + ` `) y ejecuta los siguientes comandos:

1. **forzar  crear subir todo crear commit completo**

```
git add .
git commit -m "Versión local definitiva"

```

​	**Subir los cambios forzando la sobrescritura en GitHub**

```
git push -f origin master:main

```

* git push -f origin master
* git push -f origin main



**Verificar**

* (`git remote -v`) nos dirá **a qué URL exacta de GitHub** se están subiendo tus archivos.

git log -n 1

* **la fecha y hora del último cambio** que Git tiene registrado en esa carpeta.

git remote -v

