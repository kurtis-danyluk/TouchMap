#!/usr/bin/env python
import sys

def interleave(file1, file2):
	f1 = open(file1).readlines()
	f2 = open(file2).readlines()
	fo = open("TrialsT.txt", "w+")
	for i in range(0, len(f1)):
		if i < len(f1):
			fo.write(f1[i])
			print(f1[i])
		if i < len(f2):	
			fo.write(f2[i])
			print(f1[i])
	fo.close()
	

print(sys.argv[1])	
print(sys.argv[2])	
interleave(sys.argv[1], sys.argv[2])