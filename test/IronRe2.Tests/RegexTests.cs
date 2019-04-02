using System;
using Xunit;
using IronRe2;
using System.Collections.Generic;

namespace IronRe2.Tests
{
    public class RegexTests
    {
        [Fact]
        public void InvalidRegexThrowsException()
        {
            var ex = Assert.Throws<RegexCompilationException>(() =>
            {
                new Regex("foo (bar");
            });
            Assert.Contains("missing )", ex.Message);
            Assert.Equal("foo (bar", ex.OffendingPortion);
        }

        [Fact]
        public void RegexCreateWithPatternExposesPattern()
        {
        //Given
            var regex = new Regex(".+");

        //When
            var pattern = regex.Pattern;

        //Then
            Assert.Equal(".+", pattern);
        }

        [Fact]
        public void RegexCreateEsposesProgramSize()
        {
        //Given
            var helloRe = new Regex("hello world");
            var emptyRe = new Regex("");

        //When
            var helloSize = helloRe.ProgramSize;
            var emptySize = emptyRe.ProgramSize;

        //Then
            Assert.Equal(15, helloSize);
            Assert.Equal(4, emptySize);
        }

        [Fact]
        public void RegexCreateExposesCaptureGroupInfo()
        {
        //Given
            var regex = new Regex("(.+) (?P<foo>.*)");

        //When
            var numCaptures = regex.CaptureGroupCount;
            var fooCaptureId = regex.FindNamedCapture("foo");
            var invalidCaptureId = regex.FindNamedCapture("bar");
        
        //Then
            Assert.Equal(2, numCaptures);
            Assert.Equal(2, fooCaptureId);
            Assert.Equal(-1, invalidCaptureId);
        }

        [Theory]
        [MemberData(nameof(IsMatchData))]
        public void RegexEasyIsMatch(string pattern, string haystack, bool match)
        {
            Assert.Equal(match, Regex.IsMatch(pattern, haystack));
        }

        [Theory]
        [MemberData(nameof(IsMatchData))]
        public void RegexIsMatch(string pattern, string haystack, bool match)
        {
            using (var re = new Regex(pattern))
            {
                Assert.Equal(match, re.IsMatch(haystack));
            }
        }
        
        [Theory]
        [MemberData(nameof(FindData))]
        public void RegexEasyFind(string pattern, string haystack, int start, int end)
        {
            var match = Regex.Find(pattern, haystack);
            if (start != -1)
            {
                Assert.True(match.Matched);
                Assert.Equal(start, match.Start);
                Assert.Equal(end, match.End);
            }
            else
            {
                Assert.False(match.Matched);
            }
        }

        [Theory]
        [MemberData(nameof(FindData))]
        public void RegexFind(string pattern, string haystack, int start, int end)
        {
            using (var re = new Regex(pattern))
            {
                var match = re.Find(haystack);
                if (start != -1)
                {
                    Assert.True(match.Matched);
                    Assert.Equal(start, match.Start);
                    Assert.Equal(end, match.End);
                }
                else
                {
                    Assert.False(match.Matched);
                }
            }
        }

        [Fact]
        public void FindWithCaptures()
        {
            var pattern = @"(?P<h>hello) (?P<w>world)";
            var re = new Regex(pattern);


            var captures = re.Captures("hello world");


            Assert.Equal(3, captures.Length);
            Assert.True(captures.Matched);
            Assert.Equal(0, captures.Start);
            Assert.Equal(11, captures.End);
            Assert.True(captures[0].Matched);
            Assert.Equal(0, captures[0].Start);
            Assert.Equal(11, captures[0].End);
            Assert.True(captures[1].Matched);
            Assert.Equal(0, captures[1].Start);
            Assert.Equal(5, captures[1].End);
            Assert.True(captures[2].Matched);
            Assert.Equal(6, captures[2].Start);
            Assert.Equal(11, captures[2].End);
        }

        [Fact]
        public void FindWithOptionalCaptures()
        {
            var pattern = @" (.)(.)? ";
            var re = new Regex(pattern);

            var captures = re.Captures(" a ");

            Assert.Equal(3, captures.Length);
            Assert.True(captures.Matched);
            Assert.Equal(0, captures.Start);
            Assert.Equal(3, captures.End);
            Assert.True(captures[0].Matched);
            Assert.Equal(0, captures[0].Start);
            Assert.Equal(3, captures[0].End);
            Assert.True(captures[1].Matched);
            Assert.Equal(1, captures[1].Start);
            Assert.Equal(2, captures[1].End);
            Assert.False(captures[2].Matched);
            Assert.Equal(-1, captures[2].Start);
            Assert.Equal(-1, captures[2].End);
        }

        public static IEnumerable<object[]> IsMatchData()
        {
            yield return new object[] { ".+", "hello world", true };
            yield return new object[] { "hello", "hello world", true };
            yield return new object[] { "world", "hello world", true };
            yield return new object[] { @"\s+", "hello world", true };
            yield return new object[] { ".", "", false };
            yield return new object[] { "invalid", "i'm Ok", false };
        }

        public static IEnumerable<object[]> FindData()
        {
            yield return new object[] { @".+", "hello world", 0, 11 };
            yield return new object[] { @"\b[^\s]+\b", "hello world", 0, 5 };
            yield return new object[] { @"\b[^\s]+\b$", "hello world", 6, 11 };
            yield return new object[] { @".\b", "foo bar", 2, 3 };
            yield return new object[] { @"b", "foo bar", 4, 5 };
            yield return new object[] { @"b", "nothing to see here", -1, -1 };
        }
    }
}
