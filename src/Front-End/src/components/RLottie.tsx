import {
	forwardRef,
	memo,
	useEffect,
	useImperativeHandle,
	useRef,
} from "react";

const PUBLIC_URL = location.origin;

declare global {
	interface Window {
		RLottie: any;
	}
}

const RLottie = forwardRef(
	(
		{
			sticker,
			fileId,
			width = 160,
			height = 160,
			autoplay = true,
			loop = false,
			fromFrame,
			toFrame,
		}: {
			sticker: string;
			fileId?: string;
			width?: number;
			height?: number;
			autoplay?: boolean;
			loop?: boolean;
			fromFrame?: number;
			toFrame?: number;
		},
		ref,
	) => {
		// const [data, setData] = useState()

		const player = useRef<HTMLDivElement>(null);

		const anim = useRef<any>(null);
		const data = useRef<any>(null);

		useImperativeHandle(ref, () => ({
			playFrames(fromFrame: number, toFrame: number) {
				window.RLottie.destroy(anim.current);
				console.log("play frames", fromFrame, toFrame);

				if (fromFrame) data.current.ip = fromFrame;
				if (toFrame) data.current.op = toFrame;

				const dpr = window.devicePixelRatio;

				const options = {
					container: player.current,
					loop,
					autoplay,
					stringData: JSON.stringify(data.current),
					fileId: fileId ?? sticker,
					width: width * dpr,
					height: height * dpr,
				};
				window.RLottie.loadAnimation(options, (_anim: any) => {
					console.log("play animation", fromFrame, toFrame);
					anim.current = _anim;

					if (player.current) {
						player.current.querySelector("canvas")!.style.maxWidth =
							`${width}px`;
						player.current.querySelector("canvas")!.style.maxHeight =
							`${height}px`;
					}
				});
			},
		}));

		useEffect(() => {
			(async () => {
				if (!data.current || !fromFrame) {
					const res = await fetch(
						`${PUBLIC_URL}/Ad-Marketplace-Contest/tgs/${sticker}.json`,
					);
					data.current = await res.json();
				}

				if (fromFrame) data.current.ip = fromFrame;
				if (toFrame) data.current.op = toFrame;

				// const player = document.querySelector('lottie-player');

				// setData(window.URL.createObjectURL(_data))
				// setData("https://lottie.host/6d7dd6e2-ab92-4e98-826a-2f8430768886/NGnHQ6brWA.json")

				// player.play()

				// player.load("https://lottie.host/6d7dd6e2-ab92-4e98-826a-2f8430768886/NGnHQ6brWA.json")

				const dpr = window.devicePixelRatio;

				const options = {
					container: player.current,
					loop,
					autoplay,
					stringData: JSON.stringify(data.current),
					fileId: fileId ?? sticker,
					width: width * dpr,
					height: height * dpr,
				};
				window.RLottie.loadAnimation(options, (_anim: any) => {
					anim.current = _anim;

					if (player.current) {
						player.current.querySelector("canvas")!.style.maxWidth =
							`${width}px`;
						player.current.querySelector("canvas")!.style.maxHeight =
							`${height}px`;
					}
				});
			})();

			return () => {
				window.RLottie.destroy(anim.current);
			};
		}, [sticker, fromFrame]);

		return (
			<div className="RLottie" style={{ width, height }} ref={player}></div>
		);
	},
);

export default memo(RLottie);
