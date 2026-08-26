"use client";

import Image from "next/image";
import { useCallback, useEffect, useId, useRef, useState } from "react";
import { Button } from "@/components/ui/Button";
import { Container } from "@/components/ui/Container";
import { heroSlides } from "@/config/site";

const INTERVAL_MS = 3000;

export function HeroCarousel() {
  const [index, setIndex] = useState(0);
  const [paused, setPaused] = useState(false);
  const touchX = useRef<number | null>(null);
  const liveId = useId();
  const reduceMotion = usePrefersReducedMotion();

  const go = useCallback((next: number) => {
    const total = heroSlides.length;
    setIndex(((next % total) + total) % total);
  }, []);

  useEffect(() => {
    if (paused || reduceMotion) return;
    const timer = window.setInterval(() => go(index + 1), INTERVAL_MS);
    return () => window.clearInterval(timer);
  }, [go, index, paused, reduceMotion]);

  const slide = heroSlides[index];

  return (
    <section
      className="relative isolate min-h-[32rem] overflow-hidden bg-navy text-white md:min-h-[38rem] lg:min-h-[42rem]"
      aria-roledescription="carousel"
      aria-label="Bangalore taxi highlights"
      onMouseEnter={() => setPaused(true)}
      onMouseLeave={() => setPaused(false)}
      onTouchStart={(event) => {
        touchX.current = event.touches[0]?.clientX ?? null;
      }}
      onTouchEnd={(event) => {
        if (touchX.current == null) return;
        const dx = event.changedTouches[0].clientX - touchX.current;
        if (dx > 40) go(index - 1);
        if (dx < -40) go(index + 1);
        touchX.current = null;
      }}
    >
      {heroSlides.map((item, i) => (
        <div
          key={item.id}
          className={`absolute inset-0 transition-opacity duration-500 ${i === index ? "opacity-100" : "opacity-0"}`}
          aria-hidden={i !== index}
        >
          <Image
            src={item.image.src}
            alt={item.image.alt}
            fill
            priority={i === 0}
            sizes="100vw"
            className="object-cover object-[70%_center]"
          />
        </div>
      ))}
      <div className="hero-scrim absolute inset-0" />

      <Container className="relative z-10 flex min-h-[32rem] flex-col justify-end pb-36 pt-16 md:min-h-[38rem] md:justify-center md:pb-28 lg:min-h-[42rem]">
        <p className="text-xs font-semibold uppercase tracking-[0.22em] text-taxi">{slide.eyebrow}</p>
        <h1 id={liveId} className="mt-3 max-w-xl font-display text-4xl font-semibold leading-[1.12] tracking-tight sm:text-5xl lg:text-[3.25rem]">
          {slide.title}
        </h1>
        <p className="mt-4 max-w-md text-base leading-relaxed text-white/80 sm:text-lg">{slide.text}</p>
        <div className="mt-7 flex flex-wrap gap-3">
          <Button href="/#book" variant="taxi">
            Book a cab
          </Button>
          <Button href="/#contact" variant="secondary">
            Call now
          </Button>
        </div>
        <ul className="mt-8 hidden gap-6 text-xs uppercase tracking-[0.14em] text-white/70 sm:flex">
          <li>24/7 desk</li>
          <li>Assigned drivers</li>
          <li>Airport · City · Outstation</li>
        </ul>
      </Container>

      <div className="absolute bottom-28 right-4 z-10 flex items-center gap-2 md:bottom-32 md:right-10">
        <button
          type="button"
          className="grid h-10 w-10 place-items-center rounded-sm border border-white/30 bg-navy/40 text-white"
          aria-label="Previous slide"
          onClick={() => go(index - 1)}
        >
          ‹
        </button>
        <button
          type="button"
          className="grid h-10 w-10 place-items-center rounded-sm border border-white/30 bg-navy/40 text-white"
          aria-label="Next slide"
          onClick={() => go(index + 1)}
        >
          ›
        </button>
      </div>
      <div className="absolute bottom-28 left-4 z-10 flex gap-2 md:bottom-32 md:left-[max(1rem,calc((100vw-72rem)/2+1.5rem))]">
        {heroSlides.map((item, i) => (
          <button
            key={item.id}
            type="button"
            aria-label={`Show slide: ${item.eyebrow}`}
            aria-current={i === index}
            className={`h-1.5 rounded-full transition ${i === index ? "w-8 bg-taxi" : "w-3 bg-white/40"}`}
            onClick={() => go(i)}
          />
        ))}
      </div>
    </section>
  );
}

function usePrefersReducedMotion() {
  const [reduce, setReduce] = useState(false);
  useEffect(() => {
    const media = window.matchMedia("(prefers-reduced-motion: reduce)");
    setReduce(media.matches);
    function onChange() {
      setReduce(media.matches);
    }
    media.addEventListener("change", onChange);
    return () => media.removeEventListener("change", onChange);
  }, []);
  return reduce;
}
