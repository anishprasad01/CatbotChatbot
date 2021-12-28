//UserInfoCache class
//This class is used to cache user info to preserve state as questions are asked of them.

using System.Collections.Generic;

public class UserInfoCache
{
	//User's name for saying hello
	public string Name { get; set; }
	//the state of the cat they're "building"
	public Cat CurrentCat { get; set; }
	//list of cats. this is meant to be use to preserve state long-term for future development
	//it is already in use though, and can be expended upon with future development
	public List<Cat> CatBuilds { get; set; } = new List<Cat>(); 
}
