﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudContainer.Layouter;
using TagsCloudContainer.Preprocessing;
using TagsCloudContainer.Settings;

namespace TagsCloudContainer.Tests
{
    public class LayoterApplicator_Should
    {
        private Func<CircularCloudLayouter> layouter;
        private WordInfo[] wordInfos;

        [SetUp]
        public void SetUp()
        {
            layouter = () => new CircularCloudLayouter(new Spiral(new SpiralSettings()));
            var wordInfosList = new List<WordInfo>();
            for (var i = 1; i <= 20; i++)
                wordInfosList.Add(new WordInfo
                {
                    Frequency = i,
                    Word = new string('a', i)
                });

            wordInfos = wordInfosList.ToArray();
        }

        [Test]
        public void Apply_ForEachWord()
        {
            var applicator = new LayouterApplicator(layouter, new FontSettings());

            var result = applicator.GetWordsAndRectangles(wordInfos);

            result.GetValueOrThrow().Length.Should().Be(wordInfos.Length);
        }

        [TestCase(2.1f)]
        [TestCase(10.2f)]
        [TestCase(55.5f)]
        public void SetCorrect_FontSize(float fontFactor)
        {
            var applicator = new LayouterApplicator(layouter, new FontSettings
            {
                FontSizeFactor = fontFactor
            });

            var result = applicator.GetWordsAndRectangles(wordInfos);

            foreach (var info in result.GetValueOrThrow())
                info.FontSize.Should().Be(info.Frequency * fontFactor);
        }

        [Test]
        public void BeNotSuccess_WhenNullArgument()
        {
            var applicator = new LayouterApplicator(layouter, new FontSettings());

            var result = applicator.GetWordsAndRectangles(null);

            result.IsSuccess.Should().BeFalse();
        }

        [Test]
        public void SetCenter()
        {
            var applicator = new LayouterApplicator(layouter, new FontSettings());

            applicator.GetWordsAndRectangles(wordInfos);

            applicator.WordsCenter.Should().Be(layouter().Center);
        }
    }
}