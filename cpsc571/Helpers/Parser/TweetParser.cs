using OpenNLP.Tools;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


// If you need more models go to: https://github.com/AlexPoint/OpenNlp/tree/master/Resources/Models/Parser
// For speech tags relevant to posTagging https://www.ling.upenn.edu/courses/Fall_2003/ling001/penn_treebank_pos.html
// 
namespace cpsc571.Helpers
{
    public class TweetParser
    {
        private String POS_MODEL_PATH = Path.Combine(HttpRuntime.BinDirectory, @"Helpers\Parser\Models\EnglishPOS.nbin");
        private String SD_MODEL_PATH = Path.Combine(HttpRuntime.BinDirectory, @"Helpers\Parser\Models\EnglishSD.nbin");
        private String TOK_MODEL_PATH = Path.Combine(HttpRuntime.BinDirectory, @"Helpers\Parser\Models\EnglishTok.nbin");
        private String TAG_DICT_PATH = Path.Combine(HttpRuntime.BinDirectory, @"Helpers\Parser\Models\Parser\tagdict");
        // Removed "VBN", "VBP", "VBZ"
        private String[] ALLOWED_TAGS = { "CD", "JJ", "JJR", "JJS", "MD", "NN", "NNS", "NNP", "NNPS", "RB", "RBR", "RBS", "UH", "VB", "VBD", "VBG"};
        private String pattern = @"[^0-9a-zA-Z\s']+";

        public List<String> ParseTweet(String tweetText)
        {
            List<String> result = new List<string>();

            tweetText = tweetText.Replace("i ", "I ");
            EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(TOK_MODEL_PATH);
            string[] tokens = tokenizer.Tokenize(tweetText);
            var posTagger = new EnglishMaximumEntropyPosTagger(POS_MODEL_PATH, TAG_DICT_PATH);
            string[] wordTags = posTagger.Tag(tokens);
            for(int i=0; i<tokens.Length; i++)
            {
                if (tokens[i].StartsWith("@"))
                    continue;
                else if (Uri.IsWellFormedUriString(tokens[i], UriKind.Absolute))
                    continue;
                else
                {
                    String token = Regex.Replace(tokens[i], pattern, "");
                    if (ALLOWED_TAGS.Contains(wordTags[i]) && token.Length > 0)
                        result.Add(token);
                }
            }
            return result;
        }
    }
}