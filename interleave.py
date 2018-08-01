#!/usr/bin/env python
import sys
import random



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
	

def randTrial(file1, file2):
	f1 = open(file1).readlines()
	fo = open(file2, "w+")
	
	chList = []
	csList = []
	thList = []
	tsList = []
	whList = []
	wsList = []
	ghList = []
	gsList = []
	
	for s in f1:
		if "Canmore" in s:
			if s.endswith("h\n"):
				chList.append(s)
			else:
				csList.append(s)
		elif "Wutai" in s:
			if s.endswith("h\n"):
				thList.append(s)
			else:
				tsList.append(s)
		elif "Waialeale" in s:
			if s.endswith("h\n"):
				whList.append(s)
			else:
				wsList.append(s)				
		elif "Canyon" in s:
			if s.endswith("h\n"):
				ghList.append(s)
			else:
				gsList.append(s)
		else:
			print("Did not find proper list for:" + s)
	
	print("chList")
	print(*chList)
	print(*csList)
	
	random.shuffle(chList)
	random.shuffle(csList)
	random.shuffle(thList)
	random.shuffle(tsList)
	random.shuffle(whList)
	random.shuffle(wsList)
	random.shuffle(ghList)
	random.shuffle(gsList)
	

	
	for i in range(0, len(chList)):
		fo.write(chList[i])
		fo.write(csList[i])
		fo.write(thList[i])
		fo.write(tsList[i])
		fo.write(whList[i])
		fo.write(wsList[i])
		fo.write(ghList[i])
		fo.write(gsList[i])

	
	
	
#print(sys.argv[1])	
#print(sys.argv[2])	
#interleave(sys.argv[1], sys.argv[2])
randTrial("Trials.txt", "TrialsO.txt")