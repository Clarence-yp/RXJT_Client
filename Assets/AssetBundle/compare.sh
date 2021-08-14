#!/bin/sh
if [ $# -ne 3 ]
then
    echo Usage: compare.sh fold_name_1 fold_name_2
    exit
fi
if [ -d $1 ]; then 
	rm -rf $1
	mkdir $1
else
	mkdir $1
fi

if [ ! -d $2 ]; then
    echo $2 is not a directory or not exist.
    exit
fi

if [ ! -d $3 ]; then
    echo $3 is not a directory or not exist.
    exit
fi

fold1=$1
fold2=$(echo $2 | sed 's|\(^[^/]*\).*|\1|')
fold3=$(echo $3 | sed 's|\(^[^/]*\).*|\1|')

exclude='\.meta'
equallist=equallist.txt
updatelist=updatelist.txt
excludelist=excludelist.txt

if [ -f $equallist ]; then
	rm -rf $equallist
fi
if [ -f $updatelist ]; then
	rm -rf $updatelist
fi
if [ -f $excludelist ]; then
	rm -rf $excludelist
fi

CompareFolder()
{
    for file in $1/*
    do
		echo -e ---------------------------------------------------------------------
		echo -e "compare:"$file""
		filepath=$(echo $file | sed "s/^$fold2\//\//g")
		echo $file | grep -v $exclude
        if [ "$?" = "1" ]; then
			echo -e "file:"$filepath" skip"
			echo $filepath >> $excludelist
        elif [ -d $file ]; then
			CompareFolder $file
        elif [ -f $file ]; then
			if [ ! -L $file ]; then
				file2=$(echo $file | sed "s|^.[^/]*\(.*\)|$fold3\1|")
				if [ -f $file2 ]; then
					filemd5=`md5sum $file|awk '{print $1}'`
					file2md5=`md5sum $file2|awk '{print $1}'`
					echo -e "file:"$file" md5:"$filemd5""
					echo -e "file:"$file2" md5:"$file2md5""
					
					if [ "$filemd5"x = "$file2md5"x ]; then  
					  echo -e "file:"$filepath" is equal"
					  echo $filepath >> $equallist
					else      
					  echo -e "file:"$filepath" is modify"
					  echo $filepath >> $updatelist
					fi
				else
					echo -e "file:"$filepath" is add"
					echo $filepath >> $updatelist
				fi
            fi
        fi
    done
}

CopyFiles()
{
	while read line; do
		echo -e $line
		srcfile=$fold2$line
		destfile=$fold1$line
		if [ -f $srcfile ]; then
			destdir=${destfile%/*}
			if [ ! -d $destdir ];then
				mkdir -p $destdir
			fi
			echo -e "copy file from:"$srcfile" to:"$destfile""
			cp -avfr $srcfile $destfile
		fi
	done < $1
}

CompareFolder $fold2 $fold3
CopyFiles $updatelist
exit 


