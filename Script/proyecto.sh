#!/bin/sh

echo "Opciones:"
echo " "
echo "run -> Se ejecutara el proyecto"
echo "report -> Compilar y generar el informe del proyecto"
echo "slides -> Compilar y generar la presentación del proyecto"
echo "showReport -> Visualizar el informe del proyecto"
echo "showSlides -> Visualizar la presentación del proyecto"
echo "clean -> Limpiar Ficheros innecesarios"

function run() {
cd ..
if [[ "$OSTYPE" == "linux-gnu"* ]]; then 
make dev
else
dotnet watch run --project MoogleServer
fi
}

function report() {
    cd ..
    cd "Informe"
    pdflatex Informe.tex
    pdflatex Informe.tex
}

function slides() {
    cd ..
    cd "Presentacion"
    pdflatex Presentacion.tex
    pdflatex Presentacion.tex
}

function showSlides() {
    cd ..
    cd "Presentacion"
    if ! [ -f Presentacion.pdf ]; then
        slides
    fi

    echo "Ingrese el nombre del visualizador de PDF que desea utilizar o toque enter para usar el default:"
    read viewerName
        if [[ -n "$viewerName" ]]; then
            if [[ "$OSTYPE" == "darwin"* ]]; then
                open -a "$viewerName" Presentacion.pdf
            elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
                "$vieweName" Presentacion.pdf
            else 
                start Presentacion.pdf
            fi
        else 
        if [[ "$OSTYPE" == "darwin"* ]]; then
            open Presentacion.pdf
        elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
            xdg-open Presentacion.pdf
        else 
            start Presentacion.pdf
        fi
fi
}

function showReport() {
    cd ..
    cd "Informe"
    if [[ ! -f Informe.pdf ]]; then
        report
    fi

    echo "Ingrese el nombre del visualizador de PDF que desea utilizar o toque enter para usar el default:"
    read viewerName
        if [[ -n "$viewerName" ]]; then
            if [[ "$OSTYPE" == "darwin"* ]]; then
                open -a "$viewerName" Informe.pdf
            elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
                "$vieweName" Informe.pdf
            else 
                start Informe.pdf
            fi
        else 
        if [[ "$OSTYPE" == "darwin"* ]]; then
            open Informe.pdf
        elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
            xdg-open Presentacion.pdf
        else 
            start Informe.pdf
        fi
fi
}

function clean() {
    echo "Borrando Archivos innecesarios"
    cd ..
    cd "Informe"
    rm -f Informe.pdf Informe.fdb_latexmk Informe.fls Informe.synctex.gz Informe.aux Informe.log Informe.out Informe.toc 
    cd ..
    cd "Presentacion"
    rm -f Presentacion.aux Presentacion.log Presentacion.out Presentacion.toc Presentacion.fls Presentacion.nav Presentacion.snm Presentacion.fdb_latexmk Presentacion.pdf Presentacion.synctanx.gz
}

case $1 in
    run)
        run
        ;;
    report)
        report
        ;;
    slides)
        slides 
        ;;
    showSlides)
        showSlides 
        ;;
    showReport)
        showReport 
        ;;
    clean)
        clean
        ;;
    *)
        echo "Invalid option. Usage: ./script.sh [run|report|slides|showSlides|showReport|clean]"
        exit 1
        ;;
esac



